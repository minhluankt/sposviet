using Application.Interfaces.Contexts;
using Application.Interfaces.Shared;
using AspNetCoreHero.Abstractions.Domain;

using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Model;
using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Yanga.Module.EntityFrameworkCore.AuditTrail;

namespace Infrastructure.Infrastructure.DbContexts
{
   // public partial class ApplicationDbContext : AuditableContext, IApplicationDbContext
    public partial class ApplicationDbContext : AuditableLogContext, IApplicationDbContext
    {
        private readonly IDateTimeService _dateTime;
        private readonly IAuthenticatedUserService _authenticatedUser;
        private IDbContextTransaction _transaction;
        private readonly ILogger<ApplicationDbContext> _logger;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
             ILogger<ApplicationDbContext> logger,
            IDateTimeService dateTime, IAuthenticatedUserService authenticatedUser) : base(options)
        {
            _logger = logger;
            _dateTime = dateTime;
            _authenticatedUser = authenticatedUser;
        }
        public IDbConnection Connection => throw new NotImplementedException();
        public bool HasChanges => ChangeTracker.HasChanges();
        public DbSet<TemplateInvoice> TemplateInvoice { get; set; }
        public DbSet<Post> Post { get; set; }
        public DbSet<Unit> Unit { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<StyleProduct> StyleProduct { get; set; }
        public DbSet<OptionsName> OptionsName { get; set; }
        public DbSet<StyleOptionsProduct> StyleOptionsProduct { get; set; }
        public DbSet<OptionsDetailtProduct> OptionsDetailtProduct { get; set; }
        public DbSet<ContentPromotionProduct> ContentPromotionProduct { get; set; }
        public DbSet<NotifiUser> NotifiUser { get; set; }
        public DbSet<Comment> Comment { get; set; }
        public DbSet<PromotionRun> PromotionRun { get; set; }
        public DbSet<Banner> Banner { get; set; }
        public DbSet<Brand> Brand { get; set; }
        public DbSet<PagePost> PagePost { get; set; }
        public DbSet<TableLink> TableLink { get; set; }
        public DbSet<TypeCategory> TypeCategory { get; set; }
        public DbSet<CategoryProduct> CategoryProduct { get; set; }
        public DbSet<CategoryPost> CategoryPost { get; set; }
        // public DbSet<LogSerilog> LogSerilog { get; set; }
        public DbSet<ParametersEmail> ParametersEmail { get; set; }
        public DbSet<MailSettings> MailSetting { get; set; }
        public DbSet<Mailhistory> Mailhistory { get; set; }
        public DbSet<Consultation> Consultation { get; set; }
        public DbSet<ReSearch> ReSearch { get; set; }
        public DbSet<HistoryReSearch> HistoryReSearch { get; set; }
        public DbSet<RevenueExpenditure> RevenueExpenditure { get; set; }
        public DbSet<CategoryCevenue> CategoryCevenue { get; set; }

        /// <summary>
        /// ///////
        /// </summary>
        public DbSet<StatusOrder> StatusOrder { get; set; }
        public DbSet<Customer> Customer { get; set; }
        public DbSet<ManagerIdCustomer> ManagerIdCustomer { get; set; }// quản lý số khách hàng 000001
        public DbSet<CompanyAdminInfo> CompanyAdminInfo { get; set; }
        public DbSet<ConfigSystem> ConfigSystem { get; set; }
        public DbSet<NotificationNewsEmail> NotificationNewsEmail { get; set; }
        /// <summary>
        /// ////////
        // đơn hàng
        public DbSet<DeliveryCompany> DeliveryCompany { get; set; }
        public DbSet<BankAccount> BankAccount { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<OrderDetailts> OrderDetailts { get; set; }
        public DbSet<Cart> Cart { get; set; }
        public DbSet<CartDetailt> CartDetailt { get; set; }
        public DbSet<PaymentMethod> PaymentMethod { get; set; }
        // end
        public DbSet<City> City { get; set; }
        public DbSet<District> District { get; set; }
        public DbSet<Ward> Ward { get; set; }
        public DbSet<UploadImgProduct> UploadImgProduct { get; set; }
        public DbSet<Specifications> Specifications { get; set; }
        public DbSet<TypeSpecifications> TypeSpecifications { get; set; }
        public DbSet<Document> Document { get; set; }

        // dành phần mềm bán hàng
        public DbSet<ManagerPatternEInvoice> ManagerPatternEInvoice { get; set; }
        public DbSet<SupplierEInvoice> SupplierEInvoice { get; set; }
        public DbSet<EInvoice> EInvoice { get; set; }
        public DbSet<HistoryEInvoice> HistoryEInvoice { get; set; }
        public DbSet<EInvoiceItem> EInvoiceItem { get; set; }
        public DbSet<Invoice> Invoice { get; set; }
        public DbSet<ManagerInvNo> ManagerInvNo { get; set; }
        public DbSet<InvoiceItem> InvoiceItem { get; set; }
        public DbSet<HistoryInvoice> HistoryInvoice { get; set; }
        public DbSet<RoomAndTable> RoomAndTable { get; set; }
        public DbSet<OrderTable> OrderTable { get; set; }
        public DbSet<OrderTableItem> OrderTableItem { get; set; }
        public DbSet<HistoryOrder> HistoryOrder { get; set; }
        public DbSet<Kitchen> Kitchen { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrder { get; set; }
        public DbSet<ItemPurchaseOrder> ItemPurchaseOrder { get; set; }
        public DbSet<Suppliers> Suppliers { get; set; }
        public DbSet<AdJusPaymentSupplier> AdJusPaymentSupplier { get; set; }
        public DbSet<Area> Area { get; set; }
        public DbSet<ComponentProduct> ComponentProduct { get; set; }



        /// </summary>
        public void BeginTransaction()
        {
            _transaction = Database.BeginTransaction();
        }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {

            foreach (var entry in ChangeTracker.Entries<AuditableEntity>().ToList())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        //entry.Entity.CreatedOn = _dateTime.NowUtc;
                        entry.Entity.CreatedOn = _dateTime.Now;
                        entry.Entity.CreatedBy = _authenticatedUser.UserId;
                        break;

                    case EntityState.Modified:
                        entry.Entity.LastModifiedOn = _dateTime.Now;
                        entry.Entity.LastModifiedBy = _authenticatedUser.UserId;
                        break;
                }
            }
            // int resurlt = 0;
            if (_authenticatedUser.UserId == null)
            {
                return await base.SaveChangesAsync(cancellationToken);
            }
            else
            {
                return await base.SaveChangesAsync(_authenticatedUser.ComId, _authenticatedUser.UserId);
                //return await base.SaveChangesAsync(_authenticatedUser.UserId);
            }
        }

        public void Rollback()
        {
            _transaction.Rollback();
            _transaction.Dispose();
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            foreach (var property in builder.Model.GetEntityTypes()
            .SelectMany(t => t.GetProperties())
            .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetColumnType("decimal(18,3)");
            }

            builder.Entity<Area>(entity =>
            {
                // Tạo Index Unique trên 1 cột
                entity.HasIndex(p => new { p.Slug, p.ComId }).IsUnique();
                entity.HasMany(x => x.RoomAndTables).WithOne(x => x.Area).HasForeignKey(x => x.IdArea).OnDelete(deleteBehavior: DeleteBehavior.SetNull);
            });
            builder.Entity<Suppliers>(entity =>
            {
                // Tạo Index Unique trên 1 cột
                entity.HasIndex(p => new { p.Slug, p.ComId }).IsUnique();
                entity.HasIndex(p => new { p.CodeSupplier, p.ComId }).IsUnique();
                entity.HasMany(x => x.PurchaseOrders).WithOne(x => x.Suppliers).HasForeignKey(x => x.IdSuppliers).OnDelete(deleteBehavior: DeleteBehavior.Cascade);
                entity.HasMany(x => x.AdJusPaymentSuppliers).WithOne(x => x.Suppliers).HasForeignKey(x => x.IdSuppliers).OnDelete(deleteBehavior: DeleteBehavior.Cascade);
            });
            builder.Entity<AdJusPaymentSupplier>(entity =>
            {
                // Tạo Index Unique trên 1 cột
                entity.HasIndex(p => new { p.Code, p.ComId }).IsUnique();
                entity.HasOne(x => x.Suppliers).WithMany(x => x.AdJusPaymentSuppliers).HasForeignKey(x => x.IdSuppliers).OnDelete(deleteBehavior: DeleteBehavior.NoAction);
            });
            builder.Entity<Unit>(entity =>
            {
                // Tạo Index Unique trên 1 cột
                entity.HasIndex(p => new { p.Code, p.ComId }).IsUnique();
            });
            builder.Entity<ManagerInvNo>(entity =>
            {
                entity.HasIndex(p => new { p.VFkey }).IsUnique();
            });
            builder.Entity<TemplateInvoice>(entity =>
            {
                // Tạo Index Unique trên 1 cột
                entity.HasIndex(p => new { p.Slug, p.ComId }).IsUnique();
            });
            builder.Entity<Kitchen>(entity =>
            {
                // Tạo Index Unique trên 1 cột
                entity.HasIndex(p => new { p.IdKitchen, p.ComId }).IsUnique();
                entity.HasMany(x => x.DetailtKitchens).WithOne(x => x.Kitchen).HasForeignKey(x => x.IdKitchen).OnDelete(deleteBehavior: DeleteBehavior.Cascade);
            });
            builder.Entity<DetailtKitchen>(entity =>
            {
                // Tạo Index Unique trên 1 cột
                entity.HasOne(x => x.Kitchen).WithMany(x => x.DetailtKitchens).HasForeignKey(x => x.IdKitchen).OnDelete(deleteBehavior: DeleteBehavior.NoAction);
            });
            builder.Entity<ItemPurchaseOrder>(entity =>
            {
                // Tạo Index Unique trên 1 cột
                entity.HasOne(x => x.PurchaseOrder).WithMany(x => x.ItemPurchaseOrders).HasForeignKey(x => x.IdPurchaseOrder);
            });

            builder.Entity<PurchaseOrder>(entity =>
            {
                // Tạo Index Unique trên 1 cột
                entity.HasIndex(p => new { p.Code,p.Comid}).IsUnique();
                entity.HasMany(x => x.ItemPurchaseOrders).WithOne(x => x.PurchaseOrder).HasForeignKey(x => x.IdPurchaseOrder).OnDelete(deleteBehavior: DeleteBehavior.Cascade);
                entity.HasOne(x => x.Invoice).WithMany(x => x.PurchaseOrders).HasForeignKey(x => x.IdInvoice);
                entity.HasOne(x => x.Suppliers).WithMany(x => x.PurchaseOrders).HasForeignKey(x => x.IdSuppliers).OnDelete(deleteBehavior: DeleteBehavior.NoAction);
            });
            builder.Entity<OrderTable>(entity =>
           {
               // Tạo Index Unique trên 1 cột
               entity.HasIndex(p => new { p.TypeInvoice});
               entity.HasIndex(p => new { p.TypeProduct });
               entity.HasIndex(p => new { p.ComId });
               entity.HasIndex(p => new { p.OrderTableCode, p.ComId }).IsUnique();
               entity.HasIndex(p => new { p.IdGuid }).IsUnique();
               entity.HasMany(x => x.OrderTableItems).WithOne(x => x.OrderTable).HasForeignKey(x => x.IdOrderTable).OnDelete(deleteBehavior: DeleteBehavior.Cascade);
               entity.HasMany(x => x.HistoryOrders).WithOne(x => x.OrderTable).HasForeignKey(x => x.IdOrderTable).OnDelete(deleteBehavior: DeleteBehavior.Cascade);
               entity.HasOne(x => x.Customer).WithMany(x => x.OrderTables).HasForeignKey(x => x.IdCustomer);
               entity.HasOne(x => x.RoomAndTable).WithMany(x => x.OrderTables).HasForeignKey(x => x.IdRoomAndTable);
           });

            builder.Entity<HistoryOrder>(entity =>
            {

                // Tạo Index Unique trên 1 cột TypeKitchenOrder
                entity.HasOne(x => x.OrderTable).WithMany(x => x.HistoryOrders).HasForeignKey(x => x.IdOrderTable);
            });
            builder.Entity<OrderTableItem>(entity =>
            {
                entity.HasIndex(p => new { p.IdGuid }).IsUnique();
                // Tạo Index Unique trên 1 cột
                entity.HasOne(x => x.OrderTable).WithMany(x => x.OrderTableItems).HasForeignKey(x => x.IdOrderTable);
                entity.HasMany(x => x.ToppingsOrders).WithOne(x => x.OrderTableItem).HasForeignKey(x => x.IdOrderTableItem);
            });
            builder.Entity<ToppingsOrder>(entity =>
            {
                entity.HasIndex(p => new { p.Id }).IsUnique();
                // Tạo Index Unique trên 1 cột
                entity.HasOne(x => x.OrderTableItem).WithMany(x => x.ToppingsOrders).HasForeignKey(x => x.IdOrderTableItem);
            });
            builder.Entity<RoomAndTable>(entity =>
            {
                entity.HasIndex(p => new { p.IdGuid, p.ComId }).IsUnique();
                entity.HasIndex(p => new {  p.Slug, p.ComId,p.IdArea }).IsUnique();
                // Tạo Index Unique trên 1 cột
                entity.HasMany(x => x.OrderTables).WithOne(x => x.RoomAndTable).HasForeignKey(x => x.IdRoomAndTable);
                entity.HasMany(x => x.Invoices).WithOne(x => x.RoomAndTable).HasForeignKey(x => x.IdRoomAndTable);
                entity.HasOne(x => x.Area).WithMany(x => x.RoomAndTables).HasForeignKey(x => x.IdArea);
            });
             builder.Entity<ManagerPatternEInvoice>(entity =>
             {
                 entity.HasIndex(p => new { p.VFkey }).IsUnique();
                 entity.HasIndex(p => new { p.TypeSupplierEInvoice});
                 entity.HasIndex(p => new { p.ComId });
            }); 
             builder.Entity<SupplierEInvoice>(entity =>
             {
                 entity.HasMany(x => x.ManagerPatternEInvoices).WithOne(x => x.SupplierEInvoice).HasForeignKey(x => x.IdSupplierEInvoice);
                 entity.HasIndex(p => new { p.TypeSupplierEInvoice, p.ComId }).IsUnique();
            }); 

            builder.Entity<EInvoice>(entity =>
            {
                entity.HasIndex(p => new {p.FkeyEInvoice }).IsUnique();
                entity.HasIndex(p => new {p.Fkey, p.ComId }).IsUnique();
                entity.HasIndex(p => new {p.MCQT, p.ComId }).IsUnique();
                entity.HasIndex(p => new {p.InvoiceCode, p.ComId }).IsUnique();
                entity.HasIndex(p => new { p.IdInvoice });
                entity.HasIndex(p => new {p.CusCode });
                entity.HasIndex(p => new {p.ComId });
                // Tạo Index Unique trên 1 cột
                entity.HasMany(x => x.EInvoiceItems).WithOne(x => x.EInvoice).HasForeignKey(x => x.IdInvoice);
            });
            builder.Entity<EInvoiceItem>(entity =>
            {
                entity.HasOne(x => x.EInvoice).WithMany(x => x.EInvoiceItems).HasForeignKey(x => x.IdInvoice);
            });
            builder.Entity<Invoice>(entity =>
            {
                //entity.HasIndex(p => new { p.IdGuid, p.ComId }).IsUnique();
                entity.HasIndex(p => new { p.TypeInvoice });
                entity.HasIndex(p => new {  p.TypeProduct});
                entity.HasIndex(p => new {  p.ComId });
                // Tạo Index Unique trên 1 cột
                entity.HasIndex(p => new { p.ComId, p.InvoiceCode }).IsUnique();
                entity.HasMany(x => x.InvoiceItems).WithOne(x => x.Invoice).HasForeignKey(x => x.IdInvoice).OnDelete(deleteBehavior: DeleteBehavior.Cascade);
                entity.HasMany(x => x.HistoryInvoices).WithOne(x => x.Invoice).HasForeignKey(x => x.IdInvoice).OnDelete(deleteBehavior: DeleteBehavior.Cascade);
                entity.HasMany(x => x.PurchaseOrders).WithOne(x => x.Invoice).HasForeignKey(x => x.IdInvoice).OnDelete(deleteBehavior: DeleteBehavior.Cascade);
                entity.HasOne(x => x.Customer).WithMany(x => x.Invoices).HasForeignKey(x => x.IdCustomer);
                entity.HasOne(x => x.RoomAndTable).WithMany(x => x.Invoices).HasForeignKey(x => x.IdRoomAndTable);
                entity.HasOne(x => x.PaymentMethod).WithMany(x => x.Invoices).HasForeignKey(x => x.IdPaymentMethod);
            });
        
              builder.Entity<InvoiceItem>(entity =>
            {
                // Tạo Index Unique trên 1 cột
                entity.HasOne(x => x.Invoice).WithMany(x => x.InvoiceItems).HasForeignKey(x => x.IdInvoice);
            });

            builder.Entity<HistoryInvoice>(entity =>
            {
                // Tạo Index Unique trên 1 cột
                entity.HasOne(x => x.Invoice).WithMany(x => x.HistoryInvoices).HasForeignKey(x => x.IdInvoice);
           });
             builder.Entity<HistoryEInvoice>(entity =>
            {
                // Tạo Index Unique trên 1 cột
                entity.HasOne(x => x.EInvoice).WithMany(x => x.HistoryEInvoices).HasForeignKey(x => x.IdEInvoice);
           });


            builder.Entity<TableLink>(entity =>
            {
                // Tạo Index Unique trên 1 cột
                entity.HasIndex(p => new { p.slug,p.tableId,p.Comid }).IsUnique();
            });
            builder.Entity<ContentPromotionProduct>(entity =>
            {
                // Tạo Index Unique trên 1 cột
                entity.HasIndex(p => new { p.IdProduct });
                entity.HasOne(p => p.Product).WithMany(p => p.ContentPromotionProducts).HasForeignKey(m => m.IdProduct).OnDelete(deleteBehavior: DeleteBehavior.NoAction);
            });

            builder.Entity<Consultation>(entity =>
            {
                // Tạo Index Unique trên 1 cột
                entity.Property(e => e.Slug).IsRequired();
            });
            builder.Entity<ParametersEmail>(entity =>
           {
               // Tạo Index Unique trên 1 cột
               entity.HasIndex(p => new { p.Key,p.ComId })
                         .IsUnique();
               entity.Property(e => e.Title).IsRequired();
               entity.Property(e => e.Value).IsRequired();
           });

            builder.Entity<Post>(entity =>
            {
                // Tạo Index Unique trên 1 cột
                entity.Property(e => e.Slug).IsRequired();
                entity.HasOne(p => p.CategoryPost).WithMany(p => p.Posts).HasForeignKey(m => m.IdCategory).OnDelete(deleteBehavior: DeleteBehavior.NoAction);
            });


            builder.Entity<CategoryCevenue>(entity =>
           {
               // Tạo Index Unique trên 1 cột
               entity.HasIndex(p => new { p.Slug,p.ComId })
                        .IsUnique();
               entity.HasMany(p => p.RevenueExpenditures).WithOne(p => p.CategoryCevenue).HasForeignKey(m => m.IdCategoryCevenue).OnDelete(deleteBehavior: DeleteBehavior.Cascade);
           });


            builder.Entity<RevenueExpenditure>(entity =>
            {
                entity.HasIndex(p => new { p.Code,p.ComId })
                          .IsUnique();
                entity.HasOne(p => p.CategoryCevenue).WithMany(p => p.RevenueExpenditures).HasForeignKey(m => m.IdCategoryCevenue);
            });

              builder.Entity<Cart>(entity =>
           {
               // Tạo Index Unique trên 1 cột
               entity.HasIndex(p => new { p.IdCustomer })
                        .IsUnique();
               entity.HasMany(p => p.CartDetailts).WithOne(p => p.Cart).HasForeignKey(m => m.IdCart).OnDelete(deleteBehavior: DeleteBehavior.Cascade);
           });


            builder.Entity<CartDetailt>(entity =>
          {
              entity.HasOne(p => p.Product).WithMany(p => p.CartDetailts).HasForeignKey(m => m.IdProduct).OnDelete(deleteBehavior: DeleteBehavior.SetNull);
          });



            builder.Entity<Order>(entity =>
            {
                // Tạo Index Unique trên 1 cột
                entity.HasIndex(p => new { p.OrderCode })
                          .IsUnique();
                entity.HasMany(p => p.OrderDetailts).WithOne(p => p.Order).HasForeignKey(m => m.IdOrder).OnDelete(deleteBehavior: DeleteBehavior.Cascade);
                entity.HasOne(p => p.PaymentMethod).WithMany(p => p.Order).HasForeignKey(x => x.IdPaymentMethod).OnDelete(deleteBehavior: DeleteBehavior.NoAction);
                entity.HasOne(p => p.BankAccount).WithMany(p => p.Order).HasForeignKey(x => x.IdBankAccount).OnDelete(deleteBehavior: DeleteBehavior.NoAction);
                entity.HasOne(p => p.DeliveryCompany).WithMany(p => p.Order).HasForeignKey(x => x.IdDeliveryCompany).OnDelete(deleteBehavior: DeleteBehavior.NoAction);
                entity.HasOne(p => p.Customer).WithMany(p => p.Orders).HasForeignKey(x => x.IdCustomer).OnDelete(deleteBehavior: DeleteBehavior.NoAction);

            });


            builder.Entity<BankAccount>(entity =>
            {
                // Tạo Index Unique trên 1 cột
                entity.HasIndex(p => new { p.BankNumber })
                          .IsUnique();

            });

            builder.Entity<PaymentMethod>(entity =>
         {
             // Tạo Index Unique trên 1 cột
             entity.HasIndex(p => new { p.Vkey })
                    .IsUnique();

         });


            builder.Entity<OrderDetailts>(entity =>
          {
              entity.HasOne(p => p.Product).WithMany(p => p.OrderDetailts).HasForeignKey(m => m.IdProduct).OnDelete(deleteBehavior: DeleteBehavior.SetNull);
          });




            builder.Entity<Brand>(entity =>
           {
               // Tạo Index Unique trên 1 cột
               entity.Property(e => e.Name).IsRequired();
               entity.HasMany(p => p.Products).WithOne(p => p.Brand).HasForeignKey(m => m.IdBrand).OnDelete(deleteBehavior: DeleteBehavior.SetNull);
           });

            builder.Entity<NotificationNewsEmail>(entity =>
           {
               entity.HasIndex(p => new { p.Email })
                      .IsUnique();

           }); builder.Entity<City>(entity =>
         {
             // Tạo Index Unique trên 1 cột
             entity.Property(e => e.Code).IsRequired();
             entity.Property(e => e.Name).IsRequired();
             entity.HasIndex(p => new { p.Code })
                    .IsUnique();

             entity.HasMany(p => p.Customers).WithOne(p => p.City).HasForeignKey(m => m.IdCity).OnDelete(deleteBehavior: DeleteBehavior.NoAction);
             entity.HasMany(p => p.Districts).WithOne(p => p.City).HasForeignKey(m => m.idCity).OnDelete(deleteBehavior: DeleteBehavior.Cascade);

         });
            builder.Entity<District>(entity =>
            {
                entity.Property(e => e.Name).IsRequired();
                // Tạo Index Unique trên 1 cột
                //entity.Property(e => e.Code).IsRequired();
                //entity.HasIndex(p => new { p.Name })
                //  .IsUnique();
                entity.HasMany(p => p.Customers).WithOne(p => p.District).HasForeignKey(m => m.IdDistrict).OnDelete(deleteBehavior: DeleteBehavior.SetNull);
                entity.HasMany(p => p.Wards).WithOne(p => p.District).HasForeignKey(m => m.idDistrict).OnDelete(deleteBehavior: DeleteBehavior.Cascade);
                entity.HasOne(p => p.City).WithMany(p => p.Districts).HasForeignKey(m => m.idCity).OnDelete(deleteBehavior: DeleteBehavior.NoAction);

            });
            builder.Entity<Ward>(entity =>
          {
              // Tạo Index Unique trên 1 cột
              entity.Property(e => e.Name).IsRequired();
              //entity.HasIndex(p => new { p.Name })
              //       .IsUnique();
              entity.HasMany(p => p.Customers).WithOne(p => p.Ward).HasForeignKey(m => m.IdWard).OnDelete(deleteBehavior: DeleteBehavior.NoAction);
              entity.HasOne(p => p.District).WithMany(p => p.Wards).HasForeignKey(m => m.idDistrict).OnDelete(deleteBehavior: DeleteBehavior.NoAction);
              entity.HasOne(p => p.City).WithMany(p => p.Wards).HasForeignKey(m => m.idCity).OnDelete(deleteBehavior: DeleteBehavior.NoAction);

          });

            builder.Entity<Customer>(entity =>
            {
                // Tạo Index Unique trên 1 cột
                entity.HasIndex(p => new { p.Id });
                entity.HasIndex(p => new { p.ProviderKey }).IsUnique();
                entity.HasIndex(p => new { p.Code,p.Comid }).IsUnique();
                entity.HasMany(m => m.Comments).WithOne(m => m.Customer).HasForeignKey(m => m.IdCustomer).OnDelete(deleteBehavior: DeleteBehavior.NoAction);
            });
            builder.Entity<Permission>(entity =>
            {
                // Tạo Index Unique trên 1 cột
                entity.HasIndex(p => new { p.Code })
                      .IsUnique();
            });
            builder.Entity<ConfigSystem>(entity =>
            {
                // Tạo Index Unique trên 1 cột
                entity.HasIndex(p => new { p.Key,p.ComId })
                      .IsUnique();
            });
            builder.Entity<Comment>(entity =>
            {
                // Tạo Index Unique trên 1 cột
                entity.HasIndex(p => new { p.Id });
                entity.HasOne(m => m.Product).WithMany(p => p.Comments).HasForeignKey(s => s.IdProduct).OnDelete(deleteBehavior: DeleteBehavior.NoAction);

            });



            builder.Entity<CategoryPost>(entity =>
            {
                // Tạo Index Unique trên 1 cột
                entity.HasOne(p => p.TypeCategory)
                .WithMany(g => g.Categorys)
                .HasForeignKey(s => s.IdTypeCategory)
                .IsRequired();

                entity.HasMany(p => p.Posts)
                .WithOne(p => p.CategoryPost)
                .HasForeignKey(m => m.IdCategory).OnDelete(deleteBehavior: DeleteBehavior.Cascade);

                entity.HasMany(p => p.CategoryChilds)
                .WithOne(p => p.CategoryChild)
                .HasForeignKey(m => m.IdPattern).OnDelete(deleteBehavior: DeleteBehavior.NoAction);
                // bắt buộc dữ liêu phải map đúng
            });
            builder.Entity<CategoryProduct>(entity =>
           {

               entity.HasIndex(p => new { p.ComId ,p.Code}).IsUnique();
               entity.HasMany(p => p.Products)
               .WithOne(p => p.CategoryProduct)
               .HasForeignKey(m => m.IdCategory).OnDelete(deleteBehavior: DeleteBehavior.Cascade);

               entity.HasMany(p => p.CategoryChilds)
               .WithOne(p => p.CategoryChild)
               .HasForeignKey(m => m.IdPattern).OnDelete(deleteBehavior: DeleteBehavior.NoAction);
               // bắt buộc dữ liêu phải map đúng
           });

            //builder.Entity<Category>()
            //    .HasOne(p => p.TypeCategory)
            //    .WithMany(g => g.Categorys)
            //    .HasForeignKey(s => s.IdTypeCategory)
            //    .IsRequired(); // bắt buộc dữ liêu phải map đúng


            builder.Entity<TypeCategory>(entity =>
            {
                // Tạo Index Unique trên 1 cột
                entity.HasIndex(p => new { p.Code })
                .IsUnique();
            });


            //////////////////////


            builder.Entity<CompanyAdminInfo>(entity =>
            {
                // Tạo Index Unique trên 1 cột
                entity.HasIndex(p => new {  p.VFkeyPhone })
                .IsUnique();   
                entity.HasIndex(p => new { p.VFkeyCusTaxCode })
                .IsUnique();
            });



            //////
            builder.Entity<Product>(entity =>
            {

                entity.HasIndex(p => new { p.Code,p.ComId }).IsUnique();

                entity.HasIndex(p => new { p.IdCategory });
                entity.HasIndex(p => new { p.ViewNumber });
                entity.HasIndex(p => new { p.isPromotion });

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(1000)
                    .IsRequired();


                entity.Property(e => e.Price)
                   .IsRequired();

                entity.HasMany(p => p.UploadImgProducts)
                     .WithOne(d => d.Product)
                     .HasForeignKey(d => d.IdProduct).OnDelete(deleteBehavior: DeleteBehavior.Cascade);


                entity.HasMany(p => p.Comments)
                    .WithOne(d => d.Product)
                    .HasForeignKey(d => d.IdProduct).OnDelete(deleteBehavior: DeleteBehavior.Cascade);

                entity.HasMany(p => p.ContentPromotionProducts)
                                .WithOne(d => d.Product)
                                .HasForeignKey(d => d.IdProduct).OnDelete(deleteBehavior: DeleteBehavior.Cascade);

                entity.HasOne(p => p.CategoryProduct)
                     .WithMany(d => d.Products).HasForeignKey(d => d.IdCategory).OnDelete(deleteBehavior: DeleteBehavior.NoAction);

                entity.HasOne(p => p.Customer)
                     .WithMany(d => d.Products).HasForeignKey(d => d.IdCustomer).OnDelete(deleteBehavior: DeleteBehavior.NoAction);

                entity.HasOne(p => p.UnitType)
                       .WithMany(d => d.Products).HasForeignKey(d => d.IdUnit).OnDelete(deleteBehavior: DeleteBehavior.NoAction);
                
                entity.HasOne(p => p.Brand)
                       .WithMany(d => d.Products).HasForeignKey(d => d.IdBrand).OnDelete(deleteBehavior: DeleteBehavior.NoAction);

                entity.HasMany(p => p.StyleProducts)
                       .WithOne(d => d.Product).HasForeignKey(d => d.IdProduct).OnDelete(deleteBehavior: DeleteBehavior.Cascade);
                entity.HasMany(p => p.OptionsDetailtProducts)
                       .WithOne(d => d.Product).HasForeignKey(d => d.IdProduct).OnDelete(deleteBehavior: DeleteBehavior.Cascade);
                entity.HasMany(p => p.ComponentProducts)
                       .WithOne(d => d.Product).HasForeignKey(d => d.IdProduct).OnDelete(deleteBehavior: DeleteBehavior.Cascade);

                // Tạo Index Unique trên 1 cột
                //entity.HasMany(p => p.ProductAttachment)
                //     .WithMany(d => d.Products);

            });//////


            builder.Entity<Specifications>(entity =>
            {
                // Tạo Index Unique trên 1 cột
                //entity.HasIndex(p => new { p.Code })
                //.IsUnique();
                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode();

                entity.HasOne(p => p.TypeSpecifications)
                    .WithMany(d => d.Specifications)
                    .HasForeignKey(d => d.idTypeSpecifications);


            });
            builder.Entity<TypeSpecifications>(entity =>
            {
                // Tạo Index Unique trên 1 cột
                entity.HasIndex(p => new { p.Code })
                .IsUnique();

            });

            builder.Entity<StyleProduct>(entity =>
           {
               entity.HasIndex(p => new { p.IdProduct, p.IdStyleOptionsProduct });
               entity.HasMany(p => p.OptionsNames)
                      .WithOne(d => d.StyleProduct).HasForeignKey(d => d.IdStyleProduct).OnDelete(deleteBehavior: DeleteBehavior.Cascade);

               entity.HasOne(p => p.Product)
                    .WithMany(d => d.StyleProducts).HasForeignKey(d => d.IdProduct);

               entity.HasOne(p => p.StyleOptionsProduct)
                  .WithMany(d => d.StyleProducts).HasForeignKey(d => d.IdStyleOptionsProduct);
               // entity.HasOne(p => p.StyleOptionsProduct);
           });
            builder.Entity<ComponentProduct>(entity =>
            {
                entity.HasOne(p => p.Product)
                     .WithMany(d => d.ComponentProducts).HasForeignKey(d => d.IdProduct);
            });

            builder.Entity<OptionsName>(entity =>
          {

              entity.HasOne(p => p.StyleProduct)
                     .WithMany(d => d.OptionsNames).HasForeignKey(d => d.IdStyleProduct);


          });
            builder.Entity<OptionsDetailtProduct>(entity =>
          {

              entity.HasOne(p => p.Product)
                     .WithMany(d => d.OptionsDetailtProducts).HasForeignKey(d => d.IdProduct);


          });


            //////////
            ///

            base.OnModelCreating(builder);

            // https://docs.microsoft.com/vi-vn/ef/core/modeling/relationships?tabs=fluent-api%2Cfluent-api-simple-key%2Csimple-key .OnDelete(DeleteBehavior.Cascade); 
        }

    }
}
