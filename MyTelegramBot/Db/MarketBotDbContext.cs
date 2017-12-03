using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;


namespace MyTelegramBot
{
    public partial class MarketBotDbContext : DbContext
    {
        public virtual DbSet<Address> Address { get; set; }
        public virtual DbSet<Admin> Admin { get; set; }
        public virtual DbSet<AdminKey> AdminKey { get; set; }
        public virtual DbSet<AttachmentFs> AttachmentFs { get; set; }
        public virtual DbSet<AttachmentTelegram> AttachmentTelegram { get; set; }
        public virtual DbSet<AttachmentType> AttachmentType { get; set; }
        public virtual DbSet<Basket> Basket { get; set; }
        public virtual DbSet<BlackList> BlackList { get; set; }
        public virtual DbSet<BotInfo> BotInfo { get; set; }
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<City> City { get; set; }
        public virtual DbSet<Company> Company { get; set; }
        public virtual DbSet<Configuration> Configuration { get; set; }
        public virtual DbSet<Currency> Currency { get; set; }
        public virtual DbSet<FeedBack> FeedBack { get; set; }
        public virtual DbSet<Follower> Follower { get; set; }
        public virtual DbSet<HelpDesk> HelpDesk { get; set; }
        public virtual DbSet<HelpDeskAnswer> HelpDeskAnswer { get; set; }
        public virtual DbSet<HelpDeskAnswerAttachment> HelpDeskAnswerAttachment { get; set; }
        public virtual DbSet<HelpDeskAttachment> HelpDeskAttachment { get; set; }
        public virtual DbSet<HelpDeskInWork> HelpDeskInWork { get; set; }
        public virtual DbSet<House> House { get; set; }
        public virtual DbSet<Notification> Notification { get; set; }
        public virtual DbSet<OrderAddress> OrderAddress { get; set; }
        public virtual DbSet<OrderConfirm> OrderConfirm { get; set; }
        public virtual DbSet<OrderDeleted> OrderDeleted { get; set; }
        public virtual DbSet<OrderDone> OrderDone { get; set; }
        public virtual DbSet<OrderPayment> OrderPayment { get; set; }
        public virtual DbSet<OrderProduct> OrderProduct { get; set; }
        public virtual DbSet<Orders> Orders { get; set; }
        public virtual DbSet<OrdersInWork> OrdersInWork { get; set; }
        public virtual DbSet<OrderTemp> OrderTemp { get; set; }
        public virtual DbSet<Payment> Payment { get; set; }
        public virtual DbSet<PaymentType> PaymentType { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<ProductPhoto> ProductPhoto { get; set; }
        public virtual DbSet<ProductPrice> ProductPrice { get; set; }
        public virtual DbSet<QiwiApi> QiwiApi { get; set; }
        public virtual DbSet<Raiting> Raiting { get; set; }
        public virtual DbSet<Region> Region { get; set; }
        public virtual DbSet<ReportsRequestLog> ReportsRequestLog { get; set; }
        public virtual DbSet<ShipPrice> ShipPrice { get; set; }
        public virtual DbSet<Stock> Stock { get; set; }
        public virtual DbSet<Street> Street { get; set; }
        public virtual DbSet<TelegramMessage> TelegramMessage { get; set; }
        public virtual DbSet<Units> Units { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(System.IO.Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");
            string sql = builder.Build().GetSection("Database").Value;

            //@"Server=DESKTOP-93GE9VD;Database=MarketBotDb;Integrated Security = True; Trusted_Connection = True;"

            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(sql);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address>(entity =>
            {
                entity.HasOne(d => d.Follower)
                    .WithMany(p => p.Address)
                    .HasForeignKey(d => d.FollowerId)
                    .HasConstraintName("FK_Adress_Follower");

                entity.HasOne(d => d.House)
                    .WithMany(p => p.Address)
                    .HasForeignKey(d => d.HouseId)
                    .HasConstraintName("FK_Address_House");
            });

            modelBuilder.Entity<Admin>(entity =>
            {
                entity.Property(e => e.DateAdd).HasColumnType("datetime");

                entity.HasOne(d => d.AdminKey)
                    .WithMany(p => p.Admin)
                    .HasForeignKey(d => d.AdminKeyId)
                    .HasConstraintName("FK_Admin_AdminKey");

                entity.HasOne(d => d.Follower)
                    .WithMany(p => p.Admin)
                    .HasForeignKey(d => d.FollowerId)
                    .HasConstraintName("FK_Admin_Follower");
            });

            modelBuilder.Entity<AdminKey>(entity =>
            {
                entity.Property(e => e.DateAdd).HasColumnType("datetime");

                entity.Property(e => e.KeyValue)
                    .HasMaxLength(256)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<AttachmentFs>(entity =>
            {
                entity.HasIndex(e => e.GuId)
                    .HasName("UQ__Attachme__A2B66B0514E78B0A")
                    .IsUnique();

                entity.HasIndex(e => e.Id)
                    .HasName("UQ__Attachme__3214EC06D860A4AD")
                    .IsUnique();

                entity.Property(e => e.Caption)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.AttachmentType)
                    .WithMany(p => p.AttachmentFs)
                    .HasForeignKey(d => d.AttachmentTypeId)
                    .HasConstraintName("FK_AttachmentFs_AttachmentType");
            });

            modelBuilder.Entity<AttachmentTelegram>(entity =>
            {
                entity.Property(e => e.FileId)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.AttachmentFs)
                    .WithMany(p => p.AttachmentTelegram)
                    .HasForeignKey(d => d.AttachmentFsId)
                    .HasConstraintName("FK_Attachment_AttachmentFs");

                entity.HasOne(d => d.BotInfo)
                    .WithMany(p => p.AttachmentTelegram)
                    .HasForeignKey(d => d.BotInfoId)
                    .HasConstraintName("FK_Attachment_BotInfo");
            });

            modelBuilder.Entity<AttachmentType>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Basket>(entity =>
            {
                entity.Property(e => e.DateAdd).HasColumnType("datetime");

                entity.HasOne(d => d.BotInfo)
                    .WithMany(p => p.Basket)
                    .HasForeignKey(d => d.BotInfoId)
                    .HasConstraintName("FK_Basket_BotInfo");

                entity.HasOne(d => d.Follower)
                    .WithMany(p => p.Basket)
                    .HasForeignKey(d => d.FollowerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Cart_Follower1");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Basket)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Cart_Product");
            });

            modelBuilder.Entity<BlackList>(entity =>
            {
                entity.Property(e => e.DateAdd).HasColumnType("datetime");

                entity.HasOne(d => d.Follower)
                    .WithMany(p => p.BlackList)
                    .HasForeignKey(d => d.FollowerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Blacklist_Follower");
            });

            modelBuilder.Entity<BotInfo>(entity =>
            {
                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Timestamp).HasColumnType("datetime");

                entity.Property(e => e.Token)
                    .HasMaxLength(500)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<City>(entity =>
            {
                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Region)
                    .WithMany(p => p.City)
                    .HasForeignKey(d => d.RegionId)
                    .HasConstraintName("FK_City_Region");
            });

            modelBuilder.Entity<Company>(entity =>
            {
                entity.Property(e => e.Chanel)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Chat)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.City)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Instagram)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Telephone)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Text)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.Vk)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Configuration>(entity =>
            {
                entity.Property(e => e.ExampleCsvFileId)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ExampleCsvFileMd5)
                    .HasColumnName("ExampleCsvFileMD5")
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.ManualFileId)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ManualFileMd5)
                    .HasColumnName("ManualFileMD5")
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.PrivateGroupChatId)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TemplateCsvFileId)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.TemplateCsvFileImd5)
                    .HasColumnName("TemplateCsvFileIMD5")
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.UserNameFaqFileId)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.BotInfo)
                    .WithMany(p => p.Configuration)
                    .HasForeignKey(d => d.BotInfoId)
                    .HasConstraintName("FK_Configuration_BotInfo");
            });

            modelBuilder.Entity<Currency>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ShortName)
                    .HasMaxLength(10)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<FeedBack>(entity =>
            {
                entity.HasIndex(e => e.OrderId)
                    .HasName("IX_Feedback")
                    .IsUnique();

                entity.Property(e => e.DateAdd).HasColumnType("datetime");

                entity.Property(e => e.Text)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.HasOne(d => d.Order)
                    .WithOne(p => p.FeedBack)
                    .HasForeignKey<FeedBack>(d => d.OrderId)
                    .HasConstraintName("FK_Feedback_Orders");

                entity.HasOne(d => d.Raiting)
                    .WithMany(p => p.FeedBack)
                    .HasForeignKey(d => d.RaitingId)
                    .HasConstraintName("FK_Feedback_Raiting");
            });

            modelBuilder.Entity<Follower>(entity =>
            {
                entity.Property(e => e.DateAdd).HasColumnType("datetime");

                entity.Property(e => e.FirstName)
                    .HasColumnName("FIrstName")
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Telephone)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UserName)
                    .HasMaxLength(500)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<HelpDesk>(entity =>
            {
                entity.Property(e => e.Text)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.Timestamp).HasColumnType("datetime");

                entity.HasOne(d => d.BotInfo)
                    .WithMany(p => p.HelpDesk)
                    .HasForeignKey(d => d.BotInfoId)
                    .HasConstraintName("FK_HelpDesk_BotInfo");

                entity.HasOne(d => d.Follower)
                    .WithMany(p => p.HelpDesk)
                    .HasForeignKey(d => d.FollowerId)
                    .HasConstraintName("FK_HelpDesk_Follower");
            });

            modelBuilder.Entity<HelpDeskAnswer>(entity =>
            {
                entity.Property(e => e.ClosedTimestamp).HasColumnType("datetime");

                entity.Property(e => e.Text)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.Timestamp).HasColumnType("datetime");

                entity.HasOne(d => d.Follower)
                    .WithMany(p => p.HelpDeskAnswer)
                    .HasForeignKey(d => d.FollowerId)
                    .HasConstraintName("FK_HelpDeskAnswer_Follower");

                entity.HasOne(d => d.HelpDesk)
                    .WithMany(p => p.HelpDeskAnswer)
                    .HasForeignKey(d => d.HelpDeskId)
                    .HasConstraintName("FK_HelpDeskAnswer_HelpDesk");
            });

            modelBuilder.Entity<HelpDeskAnswerAttachment>(entity =>
            {
                entity.HasKey(e => new { e.HelpDeskAnswerId, e.AttachmentFsId });

                entity.HasOne(d => d.AttachmentFs)
                    .WithMany(p => p.HelpDeskAnswerAttachment)
                    .HasForeignKey(d => d.AttachmentFsId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_HelpDeskAnswerAttachment_Attachment");

                entity.HasOne(d => d.HelpDeskAnswer)
                    .WithMany(p => p.HelpDeskAnswerAttachment)
                    .HasForeignKey(d => d.HelpDeskAnswerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_HelpDeskAnswerAttachment_HelpDesk");
            });

            modelBuilder.Entity<HelpDeskAttachment>(entity =>
            {
                entity.HasKey(e => new { e.HelpDeskId, e.AttachmentFsId });

                entity.HasOne(d => d.AttachmentFs)
                    .WithMany(p => p.HelpDeskAttachment)
                    .HasForeignKey(d => d.AttachmentFsId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_HelpDeskAttachment_Attachment");

                entity.HasOne(d => d.HelpDesk)
                    .WithMany(p => p.HelpDeskAttachment)
                    .HasForeignKey(d => d.HelpDeskId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_HelpDeskAttachment_HelpDesk");
            });

            modelBuilder.Entity<HelpDeskInWork>(entity =>
            {
                entity.Property(e => e.Timestamp).HasColumnType("datetime");

                entity.HasOne(d => d.Follower)
                    .WithMany(p => p.HelpDeskInWork)
                    .HasForeignKey(d => d.FollowerId)
                    .HasConstraintName("FK_HelpDeskInWork_Follower");

                entity.HasOne(d => d.HelpDesk)
                    .WithMany(p => p.HelpDeskInWork)
                    .HasForeignKey(d => d.HelpDeskId)
                    .HasConstraintName("FK_HelpDeskInWork_HelpDesk");
            });

            modelBuilder.Entity<House>(entity =>
            {
                entity.Property(e => e.Number)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.HasOne(d => d.Street)
                    .WithMany(p => p.House)
                    .HasForeignKey(d => d.StreetId)
                    .HasConstraintName("FK_House_Street");
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.DateAdd).HasColumnType("datetime");

                entity.HasOne(d => d.Follower)
                    .WithMany(p => p.Notification)
                    .HasForeignKey(d => d.FollowerId)
                    .HasConstraintName("FK_Notification_Follower");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Notification)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Notification_Product");
            });

            modelBuilder.Entity<OrderAddress>(entity =>
            {
                entity.HasKey(e => new { e.AdressId, e.OrderId });

                entity.HasOne(d => d.Adress)
                    .WithMany(p => p.OrderAddress)
                    .HasForeignKey(d => d.AdressId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderAdress_Adress");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderAddress)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderAdress_Order");

                entity.HasOne(d => d.ShipPrice)
                    .WithMany(p => p.OrderAddress)
                    .HasForeignKey(d => d.ShipPriceId)
                    .HasConstraintName("FK_OrdersAdress_ShipPrice");
            });

            modelBuilder.Entity<OrderConfirm>(entity =>
            {
                entity.Property(e => e.DateAdd).HasColumnType("datetime");

                entity.Property(e => e.Text)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.HasOne(d => d.Follower)
                    .WithMany(p => p.OrderConfirm)
                    .HasForeignKey(d => d.FollowerId)
                    .HasConstraintName("FK_OrderConfirm_Follower");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderConfirm)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK_OrderConfirm_Orders");
            });

            modelBuilder.Entity<OrderDeleted>(entity =>
            {
                entity.Property(e => e.DateAdd).HasColumnType("datetime");

                entity.Property(e => e.Text)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.HasOne(d => d.Follower)
                    .WithMany(p => p.OrderDeleted)
                    .HasForeignKey(d => d.FollowerId)
                    .HasConstraintName("FK_OrderDeleted_Follower");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDeleted)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK_OrderDeleted_Orders");
            });

            modelBuilder.Entity<OrderDone>(entity =>
            {
                entity.Property(e => e.DateAdd).HasColumnType("datetime");

                entity.HasOne(d => d.Follower)
                    .WithMany(p => p.OrderDone)
                    .HasForeignKey(d => d.FollowerId)
                    .HasConstraintName("FK_OrderDone_Follower");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDone)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK_OrderDone_Orders");
            });

            modelBuilder.Entity<OrderPayment>(entity =>
            {
                entity.HasKey(e => new { e.PaymentId, e.OrderId });

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderPayment)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderPayment_Orders");

                entity.HasOne(d => d.Payment)
                    .WithMany(p => p.OrderPayment)
                    .HasForeignKey(d => d.PaymentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderPayment_Payment");
            });

            modelBuilder.Entity<OrderProduct>(entity =>
            {
                entity.Property(e => e.DateAdd).HasColumnType("datetime");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderProduct)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderStr_Order");

                entity.HasOne(d => d.Price)
                    .WithMany(p => p.OrderProduct)
                    .HasForeignKey(d => d.PriceId)
                    .HasConstraintName("FK_OrderStr_Price");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderProduct)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_OrdersStr_Product");
            });

            modelBuilder.Entity<Orders>(entity =>
            {
                entity.Property(e => e.DateAdd).HasColumnType("datetime");

                entity.Property(e => e.Number).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Text)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.HasOne(d => d.BotInfo)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.BotInfoId)
                    .HasConstraintName("FK_Orders_BotInfo");

                entity.HasOne(d => d.Follower)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.FollowerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_Follower");

                entity.HasOne(d => d.PaymentType)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.PaymentTypeId)
                    .HasConstraintName("FK_Orders_PaymentType");
            });

            modelBuilder.Entity<OrdersInWork>(entity =>
            {
                entity.Property(e => e.Timestamp).HasColumnType("datetime");

                entity.HasOne(d => d.Follower)
                    .WithMany(p => p.OrdersInWork)
                    .HasForeignKey(d => d.FollowerId)
                    .HasConstraintName("FK_OrdersInWork_Follower");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrdersInWork)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK_OrdersInWork_Orders");
            });

            modelBuilder.Entity<OrderTemp>(entity =>
            {
                entity.Property(e => e.Text)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.HasOne(d => d.Address)
                    .WithMany(p => p.OrderTemp)
                    .HasForeignKey(d => d.AddressId)
                    .HasConstraintName("FK_OrderTemp_Address");

                entity.HasOne(d => d.BotInfo)
                    .WithMany(p => p.OrderTemp)
                    .HasForeignKey(d => d.BotInfoId)
                    .HasConstraintName("FK_OrderTemp_BotInfo");

                entity.HasOne(d => d.Follower)
                    .WithMany(p => p.OrderTemp)
                    .HasForeignKey(d => d.FollowerId)
                    .HasConstraintName("FK_OrderTemp_Follower");

                entity.HasOne(d => d.PaymentType)
                    .WithMany(p => p.OrderTemp)
                    .HasForeignKey(d => d.PaymentTypeId)
                    .HasConstraintName("FK_OrderTemp_PaymentType");
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.Property(e => e.Comment)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.DataAdd).HasColumnType("datetime");

                entity.Property(e => e.TxId)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.HasOne(d => d.PaymentType)
                    .WithMany(p => p.Payment)
                    .HasForeignKey(d => d.PaymentTypeId)
                    .HasConstraintName("FK_Payment_PaymentType");
            });

            modelBuilder.Entity<PaymentType>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.DateAdd).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PhotoId)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.PhotoUrl)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.TelegraphUrl)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Text)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Product)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Product_Category");

                entity.HasOne(d => d.Unit)
                    .WithMany(p => p.Product)
                    .HasForeignKey(d => d.UnitId)
                    .HasConstraintName("FK_Product_Units");
            });

            modelBuilder.Entity<ProductPhoto>(entity =>
            {
                entity.HasKey(e => new { e.ProductId, e.AttachmentFsId });

                entity.HasIndex(e => e.ProductId)
                    .HasName("IX_ProductPhoto");

                entity.HasOne(d => d.AttachmentFs)
                    .WithMany(p => p.ProductPhoto)
                    .HasForeignKey(d => d.AttachmentFsId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProductPhoto_AttachmentFs");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductPhoto)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProductPhoto_Product");
            });

            modelBuilder.Entity<ProductPrice>(entity =>
            {
                entity.Property(e => e.DateAdd).HasColumnType("datetime");

                entity.HasOne(d => d.Currency)
                    .WithMany(p => p.ProductPrice)
                    .HasForeignKey(d => d.CurrencyId)
                    .HasConstraintName("FK_ProductPrice_Currency");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductPrice)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Price_Product");
            });

            modelBuilder.Entity<QiwiApi>(entity =>
            {
                entity.Property(e => e.DateAdd).HasColumnType("datetime");

                entity.Property(e => e.Telephone)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Token)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.BotInfo)
                    .WithMany(p => p.QiwiApi)
                    .HasForeignKey(d => d.BotInfoId)
                    .HasConstraintName("FK_QiwiApi_BotInfo");
            });

            modelBuilder.Entity<Raiting>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<Region>(entity =>
            {
                entity.Property(e => e.Name)
                    .HasMaxLength(500)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ReportsRequestLog>(entity =>
            {
                entity.Property(e => e.DateAdd).HasColumnType("datetime");

                entity.HasOne(d => d.Follower)
                    .WithMany(p => p.ReportsRequestLog)
                    .HasForeignKey(d => d.FollowerId)
                    .HasConstraintName("FK_ReportsRequestLog_Follower");
            });

            modelBuilder.Entity<ShipPrice>(entity =>
            {
                entity.Property(e => e.ShipPriceId).HasColumnName("ShipPrice_id");

                entity.Property(e => e.ShipPriceEnable).HasColumnName("ShipPrice_enable");

                entity.Property(e => e.ShipPriceValue).HasColumnName("ShipPrice_value");
            });

            modelBuilder.Entity<Stock>(entity =>
            {
                entity.Property(e => e.DateAdd).HasColumnType("datetime");

                entity.Property(e => e.Text)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Stock)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Stock_Product");
            });

            modelBuilder.Entity<Street>(entity =>
            {
                entity.Property(e => e.Name)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.HasOne(d => d.City)
                    .WithMany(p => p.Street)
                    .HasForeignKey(d => d.CityId)
                    .HasConstraintName("FK_Street_City");
            });

            modelBuilder.Entity<TelegramMessage>(entity =>
            {
                entity.Property(e => e.DateAdd).HasColumnType("datetime");

                entity.Property(e => e.MessageId)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateJson).IsUnicode(false);

                entity.HasOne(d => d.BotInfo)
                    .WithMany(p => p.TelegramMessage)
                    .HasForeignKey(d => d.BotInfoId)
                    .HasConstraintName("FK_TelegramMessage_BotInfo");

                entity.HasOne(d => d.Follower)
                    .WithMany(p => p.TelegramMessage)
                    .HasForeignKey(d => d.FollowerId)
                    .HasConstraintName("FK_TelegramMessage_Follower");
            });

            modelBuilder.Entity<Units>(entity =>
            {
                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ShortName)
                    .HasMaxLength(5)
                    .IsUnicode(false);
            });
        }
    }
}
