using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Ratio_Lyrics.Web.Areas.Admin.Models;
using Ratio_Lyrics.Web.Areas.Admin.Models.User;
using Ratio_Lyrics.Web.Entities;
using Ratio_Lyrics.Web.Helpers;
using Ratio_Lyrics.Web.Helpers.QueryableHelpers;
using Ratio_Lyrics.Web.Models;
using Ratio_Lyrics.Web.Models.Enums;
using Ratio_Lyrics.Web.Repositories.Abstracts;
using Ratio_Lyrics.Web.Services.Abstraction;
using Ratio_Lyrics.Web.Services.Abstractions;
using System.Security.Claims;

namespace Ratio_Lyrics.Web.Services.Implements
{
    public class UserService : IUserService
    {
        private readonly SignInManager<RatioLyricUsers> _signInManager;
        private readonly UserManager<RatioLyricUsers> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserStore<RatioLyricUsers> _userStore;
        private readonly IUserEmailStore<RatioLyricUsers> _emailStore;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;
        private readonly ICommonService _commonService;

        public UserService(UserManager<RatioLyricUsers> userManager,
            IUserStore<RatioLyricUsers> userStore,
            RoleManager<IdentityRole> roleManager,
            ILogger<UserService> logger,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ICommonService commonService,
            SignInManager<RatioLyricUsers> signInManager)
        {
            _userManager = userManager;
            _userStore = userStore;
            _roleManager = roleManager;
            _logger = logger;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _commonService = commonService;
            _userRepository = unitOfWork.UserRepository;
            _signInManager = signInManager;
            _emailStore = GetEmailStore();
        }

        public async Task<ListUsersViewModel<UserViewModel>> Gets(BaseSearchArgs args)
        {
            int totalCount = 0;
            var request = _mapper.Map<BaseSearchRequest>(args);
            var users = GetRatioLyricUsers(request, out totalCount);
            args.PageIndex = args.PageIndex <= 0 ? 1 : args.PageIndex;
            users = users.Skip((args.PageIndex - 1) * args.PageSize).Take(args.PageSize);

            var results = new List<UserViewModel>();
            foreach (var item in users.ToList())
            {
                var user = _mapper.Map<UserViewModel>(item);
                user.UserRoles = (await _userManager.GetRolesAsync(item)).ToList();
                results.Add(user);
            }            

            var result = new ListUsersViewModel<UserViewModel>
            {
                Users = results,
                PageIndex = args.PageIndex <= 0 ? 1 : args.PageIndex,
                PageSize = args.PageSize,
                FilterItems = request.FilterItems.CleanDefaultFilter(),
                SortType = args.SortType,
                IsSelectPreviousItems = args.IsSelectPreviousItems,
                TotalCount = totalCount,
                TotalPage = totalCount == 0 ? 1 : (int)Math.Ceiling((double)totalCount / args.PageSize)
            };
            return result;
        }

        public async Task<UserViewModel?> Get(string id)
        {
            var user = await _userRepository.Get(id);
            if (user == null) return null;

            var roles = await _userManager.GetRolesAsync(user);

            var results = _mapper.Map<UserViewModel>(user);
            results.UserRoles = roles.ToList();

            return results;
        }

        public async Task<RegisterResponseViewModel> CreateEmployee(UserViewModel newUser)
        {
            if (newUser == null 
                || string.IsNullOrWhiteSpace(newUser.UserName) 
                || string.IsNullOrEmpty(newUser.Password)) 
                return new RegisterResponseViewModel { Status = Constants.CommonConstant.Failure, Message = "Bad request" };

            var user = new RatioLyricUsers
            {
                DisplayName = newUser.DisplayName,
                IsClientUser = false,
                PasswordHash = _commonService.HashPasword(newUser.Password, out var salt),
                HashSalt = Convert.ToHexString(salt),
            };

            var existUser = await _userStore.FindByNameAsync(newUser.UserName.ToString().ToUpper(), CancellationToken.None);
            if(existUser != null)
            {
                _logger.LogError($"Can't create new user. Username existed: {newUser.UserName}");
                return new RegisterResponseViewModel { Status = Constants.CommonConstant.Failure, Message = "Username existed!" };
            }

            await _userStore.SetUserNameAsync(user, newUser.UserName, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, newUser.Email, CancellationToken.None);
            var result = await _userManager.CreateAsync(user);

            if (result.Succeeded)
            {
                await UpdatePhoneNumber(user, newUser.PhoneNumber);
                await _userManager.AddToRolesAsync(user, newUser.UserRoles);
                await _signInManager.SignInAsync(user, isPersistent: false);
                return new RegisterResponseViewModel
                {
                    UserName = user.UserName,
                    Status = Constants.CommonConstant.Success
                };
            }

            return new RegisterResponseViewModel { Status = Constants.CommonConstant.Failure, Message="Can't create user" };
        }

        public async Task<IdentityResult> CreateExternalUser(RatioLyricUsers user, ExternalLoginInfo? info)
        {
            string email = info.Principal.FindFirstValue(ClaimTypes.Email);

            // check username exist
            if (await _userRepository.GetAll().AsQueryable().FirstOrDefaultAsync(x => x.UserName.Equals(email)) != null)
            {
                email = $"{email}.{info.LoginProvider}".ToLower();
            }

            await _userStore.SetUserNameAsync(user, email, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, email, CancellationToken.None);

            var result = await _userManager.CreateAsync(user);
            return result;
        }

        public async Task<bool> UpdateEmployee(UserViewModel userModel)
        {
            try
            {
                await _unitOfWork.CreateTransactionAsync();
                var user = await _userRepository.Get(userModel.Id.ToString());

                if (user == null) return false;

                // update basic info
                user.PhoneNumber = userModel.PhoneNumber;
                user.DisplayName = userModel.DisplayName;
                user.Email = userModel.Email;
                await _unitOfWork.SaveAsync();

                // update role
                var currentUserRoles = await _userManager.GetRolesAsync(user);
                var isEqual = currentUserRoles?.OrderBy(x => x).ToList().SequenceEqual(userModel.UserRoles.OrderBy(x => x));
                bool updateRoleStatus = true;
                if (isEqual != true)
                {
                    await _userManager.RemoveFromRolesAsync(user, currentUserRoles);

                    var result = await _userManager.AddToRolesAsync(user, userModel.UserRoles);
                    updateRoleStatus = result.Succeeded;
                }

                // update phone number
                //bool setPhoneStatus = true;
                //if (userModel.PhoneNumber != user.PhoneNumber)
                //{
                //    var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, userModel.PhoneNumber);
                //    setPhoneStatus = setPhoneResult.Succeeded;
                //}

                if (updateRoleStatus)
                {
                    await _unitOfWork.CommitAsync();
                    return true;
                }

                _logger.LogError("Failure to update user roles");
                await _unitOfWork.RollbackAsync();
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception save admin user: {ex}");
                await _unitOfWork.RollbackAsync();
                return false;
            }
        }

        public async Task<bool> UpdatePhoneNumber(RatioLyricUsers user, string newPhoneNumber)
        {
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (newPhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, newPhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    return false;
                }
            }
            return true;
        }

        public async Task<bool> Delete(string id)
        {
            var result = await _userRepository.DeleteAsync(id);
            if (!result) return false;

            await _unitOfWork.SaveAsync();
            return result;
        }

        public async Task<List<string>>? GetUserRoles(RatioLyricUsers user)
        {
            if (user == null) return null;

            var result = await _userManager.GetRolesAsync(user);
            return result.ToList();
        }

        public async Task<List<IdentityRole>> GetRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return roles;
        }

        public async Task<LoginResponseViewModel> AdminUserLogin(LoginRequestViewModel request)
        {
            if (request == null || string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.Password)) return new LoginResponseViewModel { Status = Constants.CommonConstant.Failure };

            var userInfo = await _userRepository.GetAll().AsQueryable().FirstOrDefaultAsync(x => x.UserName.Equals(request.UserName));
            if (userInfo == null || string.IsNullOrEmpty(userInfo.PasswordHash) || string.IsNullOrEmpty(userInfo.HashSalt)) return new LoginResponseViewModel { Status = Constants.CommonConstant.Failure };

            var result = _commonService.VerifyPassword(request.Password, userInfo.PasswordHash, Convert.FromHexString(userInfo.HashSalt));
            if (!result) return new LoginResponseViewModel { Status = Constants.CommonConstant.Failure };

            await _signInManager.SignInAsync(userInfo, isPersistent: request.RememberMe);
            return new LoginResponseViewModel
            {
                UserName = request.UserName,
                Status = Constants.CommonConstant.Success
            };
        }

        private IQueryable<RatioLyricUsers>? GetRatioLyricUsers(BaseSearchRequest args, out int totalCount)
        {
            var users = _userRepository.GetAll().AsQueryable();
            users = BuildShopUserFilters(users, args);
            users = users?.OrderBy(x => x.DisplayName);

            totalCount = users?.Count() ?? 0;

            return users;
        }

        private IQueryable<RatioLyricUsers>? BuildShopUserFilters(IQueryable<RatioLyricUsers>? queries, IFacetFilter? filters)
        {
            if (queries == null || filters == null || filters.FilterItems == null || !filters.FilterItems.Any()) return queries;

            var predicate = PredicateBuilder.True<RatioLyricUsers>();
            foreach (var item in filters.FilterItems)
            {
                if (item == null) continue;

                var getfilterType = Enum.TryParse(typeof(FilterType), item.Type, true, out var filterType);
                if (!getfilterType) continue;

                switch (filterType)
                {
                    case FilterType.Text:
                        {
                            switch (item.FieldName)
                            {
                                case "":
                                    break;
                                case "Name":
                                    predicate = predicate.And(x => x.DisplayName.Contains(item.Value));
                                    break;
                                case "PhoneNumber":
                                    predicate = predicate.And(x => x.PhoneNumber.Contains(item.Value));
                                    break;
                                case "Email":
                                    predicate = predicate.And(x => x.Email.Contains(item.Value));
                                    break;
                            }
                            break;
                        }
                    case FilterType.Bool:
                        switch (item.FieldName)
                        {
                            case "IsClientUser":
                                predicate = predicate.And(x => x.IsClientUser == bool.Parse(item.Value));
                                break;
                        }
                        break;
                }
            }

            return queries.Where(predicate);
        }

        private IUserEmailStore<RatioLyricUsers> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<RatioLyricUsers>)_userStore;
        }
    }
}
