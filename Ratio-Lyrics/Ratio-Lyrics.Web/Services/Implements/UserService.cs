using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ratio_Lyrics.Web.Areas.Admin.Models;
using Ratio_Lyrics.Web.Areas.Admin.Models.User;
using Ratio_Lyrics.Web.Entities;
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
        }

        public async Task<UserViewModel?> Get(string id)
        {
            var user = await _userRepository.Get(id);
            if (user == null) return null;

            return _mapper.Map<UserViewModel>(user);
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

        public async Task<RegisterResponseViewModel> RegisterAdminUser(UserViewModel newUser)
        {
            if (newUser == null || string.IsNullOrWhiteSpace(newUser.UserName) || string.IsNullOrEmpty(newUser.Password)) return new RegisterResponseViewModel { Status = "Failure" };

            var user = new RatioLyricUsers
            {
                DisplayName = newUser.DisplayName,
                PasswordHash = _commonService.HashPasword(newUser.Password, out var salt),
                HashSalt = Convert.ToHexString(salt)
            };

            await _userStore.SetUserNameAsync(user, newUser.UserName, CancellationToken.None);
            var result = await _userManager.CreateAsync(user);

            if (result.Succeeded)
            {
                await UpdatePhoneNumber(user, newUser.PhoneNumber);
                await _userManager.AddToRoleAsync(user, Constants.CommonConstant.Admin);
                await _signInManager.SignInAsync(user, isPersistent: false);
                return new RegisterResponseViewModel
                {
                    UserName = user.UserName,
                    Status = "Success"
                };
            }

            return new RegisterResponseViewModel { Status = "Failure" };
        }

        public async Task<LoginResponseViewModel> AdminUserLogin(LoginRequestViewModel request)
        {
            if (request == null || string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.Password)) return new LoginResponseViewModel { Status = "Failure" };

            var userInfo = await _userRepository.GetAll().AsQueryable().FirstOrDefaultAsync(x => x.UserName.Equals(request.UserName));
            if (userInfo == null || string.IsNullOrEmpty(userInfo.PasswordHash) || string.IsNullOrEmpty(userInfo.HashSalt)) return new LoginResponseViewModel { Status = "Failure" };

            var result = _commonService.VerifyPassword(request.Password, userInfo.PasswordHash, Convert.FromHexString(userInfo.HashSalt));
            if (!result) return new LoginResponseViewModel { Status = "Failure" };

            await _signInManager.SignInAsync(userInfo, isPersistent: false);
            return new LoginResponseViewModel
            {
                UserName = request.UserName,
                Status = "Success"
            };
        }       

        public async Task<List<IdentityRole>> GetRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return roles;
        }

        public async Task<ListUsersViewModel<UserViewModel>> Gets(BaseSearchArgs args)
        {
            int totalCount = 0;
            var request = _mapper.Map<BaseSearchRequest>(args);
            var users = GetRatioLyricUsers(request, out totalCount);

            var usersInfo = _mapper.Map<List<UserViewModel>>(users);

            if (usersInfo != null && usersInfo.Any())
            {
                foreach (var item in usersInfo)
                {
                    var user = await Get(item.Id.ToString());
                    if (user == null) continue;

                    var roles = await GetUserRoles(user);
                    item.UserRole = String.Join(", ", roles);
                }
            }

            totalCount = usersInfo.Count;
            args.PageIndex = args.PageIndex <= 0 ? 1 : args.PageIndex;
            usersInfo = usersInfo.OrderBy(x => x.UserRole).ThenBy(x => x.FullName).Skip((args.PageIndex - 1) * args.PageSize).Take(args.PageSize).ToList();

            var result = new ListUsersViewModel<UserViewModel>
            {
                Users = usersInfo,
                PageIndex = args.PageIndex <= 0 ? 1 : args.PageIndex,
                PageSize = args.PageSize,
                FilterItems = args.FilterItems.CleanDefaultFilter(),
                SortType = args.SortType,
                IsSelectPreviousItems = args.IsSelectPreviousItems,
                TotalCount = totalCount,
                TotalPage = totalCount == 0 ? 1 : (int)Math.Ceiling((double)totalCount / args.PageSize)
            };
            return result;
        }

        public Task<string> Create(UserViewModel user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(UserViewModel user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(string id)
        {
            throw new NotImplementedException();
        }

        public Task<int> CreateExternalUser(RatioLyricUsers user, ExternalLoginInfo? info)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CheckUserNameExist(string userName)
        {
            throw new NotImplementedException();
        }

        public async Task<List<string>>? GetUserRoles(RatioLyricUsers user)
        {
            if (user == null) return null;

            var result = await _userManager.GetRolesAsync(user);
            return result.ToList();
        }

        public Task<ListUsersViewModel<UserViewModel>> GetListEmployees(BaseSearchRequest args)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CreateEmployee(UserViewModel user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateEmployee(UserViewModel user)
        {
            throw new NotImplementedException();
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
                }
            }

            return queries.Where(predicate);
        }
    }
}
