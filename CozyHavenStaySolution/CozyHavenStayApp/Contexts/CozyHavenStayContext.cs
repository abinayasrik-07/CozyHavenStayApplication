using System;
using System.Collections.Generic;
using CozyHavenStayApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CozyHavenStayApp.Contexts;

public partial class CozyHavenStayContext : DbContext
{
    public CozyHavenStayContext()
    {
    }

    public CozyHavenStayContext(DbContextOptions<CozyHavenStayContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Amenity> Amenities { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<BookingCancellation> BookingCancellations { get; set; }

    public virtual DbSet<Discount> Discounts { get; set; }

    public virtual DbSet<Hotel> Hotels { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }

    public virtual DbSet<ProofOfUser> ProofOfUsers { get; set; }

    public virtual DbSet<Refund> Refunds { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<RoomType> RoomTypes { get; set; }

    public virtual DbSet<StarRating> StarRatings { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-SA5ENHD;TrustServerCertificate=True;Integrated Security=True;Database=CozyHavenStay;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Amenity>(entity =>
        {
            entity.HasKey(e => e.AmenityId).HasName("PK__Amenitie__842AF50B19F4909D");

            entity.HasIndex(e => e.AmenityName, "UQ__Amenitie__7B4A459F081FA2BB").IsUnique();

            entity.Property(e => e.AmenityName).HasMaxLength(100);
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__Bookings__73951AEDD9312AF3");

            entity.Property(e => e.BookingStatus)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");
            entity.Property(e => e.DateOfBooking)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Room).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Bookings__RoomId__73BA3083");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Bookings__UserId__72C60C4A");
        });

        modelBuilder.Entity<BookingCancellation>(entity =>
        {
            entity.HasKey(e => e.CancellationId).HasName("PK__BookingC__6A2D9A3A149AF49A");

            entity.ToTable("BookingCancellation");

            entity.Property(e => e.CancellationDate).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.ReasonForCancellation).HasMaxLength(255);

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingCancellations)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK__BookingCa__Booki__1332DBDC");
        });

        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasKey(e => e.DiscountId).HasName("PK__Discount__E43F6D9667C20501");

            entity.HasIndex(e => e.DiscountCode, "UQ__Discount__A1120AF5FCB76227").IsUnique();

            entity.Property(e => e.AppliedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.DiscountCode).HasMaxLength(50);
            entity.Property(e => e.DiscountPercentage).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.ExpiryDate).HasColumnType("datetime");

            entity.HasOne(d => d.Booking).WithMany(p => p.Discounts)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK__Discounts__Booki__797309D9");
        });

        modelBuilder.Entity<Hotel>(entity =>
        {
            entity.HasKey(e => e.HotelId).HasName("PK__Hotels__46023BDF2871288F");

            entity.Property(e => e.HotelName).HasMaxLength(100);

            entity.HasOne(d => d.Location).WithMany(p => p.Hotels)
                .HasForeignKey(d => d.LocationId)
                .HasConstraintName("FK__Hotels__Location__5CD6CB2B");

            entity.HasOne(d => d.StarRating).WithMany(p => p.Hotels)
                .HasForeignKey(d => d.StarRatingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Hotels__StarRati__5EBF139D");

            entity.HasOne(d => d.User).WithMany(p => p.Hotels)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Hotels__UserId__5DCAEF64");

            entity.HasMany(d => d.Amenities).WithMany(p => p.Hotels)
                .UsingEntity<Dictionary<string, object>>(
                    "HotelAmenity",
                    r => r.HasOne<Amenity>().WithMany()
                        .HasForeignKey("AmenityId")
                        .HasConstraintName("FK__HotelAmen__Ameni__6E01572D"),
                    l => l.HasOne<Hotel>().WithMany()
                        .HasForeignKey("HotelId")
                        .HasConstraintName("FK__HotelAmen__Hotel__6D0D32F4"),
                    j =>
                    {
                        j.HasKey("HotelId", "AmenityId").HasName("PK__HotelAme__EE40948FDEBB95A7");
                        j.ToTable("HotelAmenities");
                    });
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.LocationId).HasName("PK__Location__E7FEA4973865DC4C");

            entity.Property(e => e.City)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Country)
                .HasMaxLength(30)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payments__9B556A38598B130F");

            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.FinalAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PaymentDate).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.PaymentStatus).HasMaxLength(50);
            entity.Property(e => e.TransactionId).HasMaxLength(100);

            entity.HasOne(d => d.Booking).WithMany(p => p.Payments)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK__Payments__Bookin__03F0984C");

            entity.HasOne(d => d.Discount).WithMany(p => p.Payments)
                .HasForeignKey(d => d.DiscountId)
                .HasConstraintName("FK__Payments__Discou__02FC7413");

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.Payments)
                .HasForeignKey(d => d.PaymentMethodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payments__Paymen__02084FDA");
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.PaymentMethodId).HasName("PK__PaymentM__DC31C1D3979609B5");

            entity.ToTable("PaymentMethod");

            entity.HasIndex(e => e.MethodName, "UQ__PaymentM__218CFB1707C5740E").IsUnique();

            entity.Property(e => e.MethodName).HasMaxLength(50);
        });

        modelBuilder.Entity<ProofOfUser>(entity =>
        {
            entity.HasKey(e => e.ProofTypeId).HasName("PK__ProofOfU__DDC9CA7D7B731BB3");

            entity.ToTable("ProofOfUser");

            entity.HasIndex(e => e.TypeName, "UQ__ProofOfU__D4E7DFA8FD685ED6").IsUnique();

            entity.Property(e => e.TypeName).HasMaxLength(20);
        });

        modelBuilder.Entity<Refund>(entity =>
        {
            entity.HasKey(e => e.RefundId).HasName("PK__Refunds__725AB9208C6E9AC1");

            entity.Property(e => e.RefundAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.RefundDate).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.RefundStatus)
                .HasMaxLength(20)
                .HasDefaultValue("Processing");

            entity.HasOne(d => d.Payment).WithMany(p => p.Refunds)
                .HasForeignKey(d => d.PaymentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Refunds__Payment__0C85DE4D");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__Reviews__74BC79CED52D0989");

            entity.Property(e => e.DatePosted).HasDefaultValueSql("(sysdatetime())");

            entity.HasOne(d => d.Booking).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK__Reviews__Booking__08B54D69");

            entity.HasOne(d => d.Hotel).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.HotelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reviews__HotelId__09A971A2");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE1A0F0DFD03");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B6160D67342C3").IsUnique();

            entity.Property(e => e.RoleName).HasMaxLength(20);
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.RoomId).HasName("PK__Rooms__3286393955291BF5");

            entity.Property(e => e.BaseFare).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.IsAc)
                .HasDefaultValue(true)
                .HasColumnName("IsAC");
            entity.Property(e => e.IsAvailable).HasDefaultValue(true);
            entity.Property(e => e.Size).HasMaxLength(50);

            entity.HasOne(d => d.Hotel).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.HotelId)
                .HasConstraintName("FK__Rooms__HotelId__66603565");

            entity.HasOne(d => d.RoomType).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.RoomTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Rooms__RoomTypeI__6754599E");
        });

        modelBuilder.Entity<RoomType>(entity =>
        {
            entity.HasKey(e => e.RoomTypeId).HasName("PK__RoomType__BCC896313F9F24DC");

            entity.ToTable("RoomType");

            entity.HasIndex(e => e.RoomTypeName, "UQ__RoomType__4A6B3769452352B4").IsUnique();

            entity.Property(e => e.RoomTypeName).HasMaxLength(50);
        });

        modelBuilder.Entity<StarRating>(entity =>
        {
            entity.HasKey(e => e.StarRatingId).HasName("PK__StarRati__76977500749CA70B");

            entity.ToTable("StarRating");

            entity.HasIndex(e => e.Rating, "UQ__StarRati__45BE90D500E316CC").IsUnique();
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4CF8C5A26C");

            entity.HasIndex(e => e.Phone, "UQ__Users__5C7E359E361B8A06").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534D52393C2").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FullName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(30);
            entity.Property(e => e.ProofTypeId).HasDefaultValueSql("(NULL)");

            entity.HasOne(d => d.ProofType).WithMany(p => p.Users)
                .HasForeignKey(d => d.ProofTypeId)
                .HasConstraintName("FK__Users__ProofType__5535A963");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users__RoleId__5629CD9C");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
