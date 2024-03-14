//using Microsoft.EntityFrameworkCore;
//using Ratio_Lyrics.Web.Data;
//using Ratio_Lyrics.Web.Entities;
//using Ratio_Lyrics.Web.Repositories.Abstracts;

//namespace Ratio_Lyrics.Web.Repositories.Implements
//{
//    public class ProductRepository : BaseRepository<Product>, IProductRepository
//    {
//        public ProductRepository(PaymentDBContext context) : base(context)
//        {
//        }

//        public async Task<Product?> GetByIdIncludeAsync(int id, bool isTracking = true)
//        {
//            var result = GetAll(isTracking).AsQueryable()
//                .Include(x => x.ProductCategories)
//                .ThenInclude(x => x.Category)
//                .FirstOrDefaultAsync(x => x.Id == id);
            
//            return await result;
//        }
//    }
//}
