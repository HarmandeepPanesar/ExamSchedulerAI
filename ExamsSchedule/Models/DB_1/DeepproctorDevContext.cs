using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ExamsSchedule.Models.DB
{
    public partial class DeepproctorDevContext : DbContext
    {
        public DeepproctorDevContext()
        {
        }

        public DeepproctorDevContext(DbContextOptions<DeepproctorDevContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AspNetRoleClaims> AspNetRoleClaims { get; set; }
        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUserRoles> AspNetUserRoles { get; set; }
        public virtual DbSet<AspNetUserTokens> AspNetUserTokens { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<DeviceLogs> DeviceLogs { get; set; }
        public virtual DbSet<DeviceMap> DeviceMap { get; set; }
        public virtual DbSet<Lmsintegration> Lmsintegration { get; set; }
        public virtual DbSet<PEventEnum> PEventEnum { get; set; }
        public virtual DbSet<PEvents> PEvents { get; set; }
        public virtual DbSet<PExam> PExam { get; set; }
        public virtual DbSet<PExamStudent> PExamStudent { get; set; }
        public virtual DbSet<PExamStudentProctoringStatus> PExamStudentProctoringStatus { get; set; }
        public virtual DbSet<PExamStudentVerification> PExamStudentVerification { get; set; }
        public virtual DbSet<PProctorComments> PProctorComments { get; set; }
        public virtual DbSet<PProctoringEnum> PProctoringEnum { get; set; }
        public virtual DbSet<PProctroingEvaluation> PProctroingEvaluation { get; set; }
        public virtual DbSet<PStudent> PStudent { get; set; }
        public virtual DbSet<PStudentExamPicture> PStudentExamPicture { get; set; }
        public virtual DbSet<PStudentFaculty> PStudentFaculty { get; set; }
        public virtual DbSet<ProctoringQueue> ProctoringQueue { get; set; }

        

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=mmu-activateazure.database.windows.net;Database=Deepproctor.Dev;User Id=mmusqllogin;Password=Test4218#;MultipleActiveResultSets=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AspNetRoleClaims>(entity =>
            {
                entity.Property(e => e.RoleId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetRoleClaims)
                    .HasForeignKey(d => d.RoleId);
            });

            modelBuilder.Entity<AspNetRoles>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(256);

                entity.Property(e => e.NormalizedName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetUserClaims>(entity =>
            {
                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserClaims)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserLogins>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

                entity.Property(e => e.LoginProvider).HasMaxLength(128);

                entity.Property(e => e.ProviderKey).HasMaxLength(128);

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserLogins)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserRoles>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.RoleId);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserTokens>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

                entity.Property(e => e.LoginProvider).HasMaxLength(128);

                entity.Property(e => e.Name).HasMaxLength(128);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserTokens)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUsers>(entity =>
            {
                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);

                entity.Property(e => e.UserName).HasMaxLength(256);
            });

            modelBuilder.Entity<DeviceLogs>(entity =>
            {
                entity.HasKey(e => e.DeviceLogId);

                entity.Property(e => e.DeviceId)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.PayLoad).IsUnicode(false);

                entity.Property(e => e.RecordedDtm).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<DeviceMap>(entity =>
            {
                entity.HasKey(e => e.DeviceClientId);

                entity.Property(e => e.ActivatedOn).HasColumnType("datetime");

                entity.Property(e => e.AssociateCompany)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.AssociatedUser)
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.Property(e => e.DeviceId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Location)
                    .HasMaxLength(1000)
                    .IsUnicode(false);
            });

            //modelBuilder.Entity<Lmsintegration>(entity =>
            //{
            //    entity.ToTable("LMSIntegration");

            //    entity.Property(e => e.Id).ValueGeneratedNever();

            //    entity.Property(e => e.SecurityToken)
            //        .HasMaxLength(10)
            //        .IsFixedLength();

            //    entity.Property(e => e.WebServiceLink)
            //        .HasMaxLength(10)
            //        .IsFixedLength();
            //});

            modelBuilder.Entity<PEventEnum>(entity =>
            {
                entity.ToTable("pEventEnum");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.EventDescription)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.EventEnum).ValueGeneratedOnAdd();

                entity.Property(e => e.EventName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<PEvents>(entity =>
            {
                entity.HasKey(e => e.EventId)
                    .HasName("PK_Events");

                entity.ToTable("pEvents");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Event)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.ExamIdentifier)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.FileName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.SessionId)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<PExam>(entity =>
            {
                entity.HasKey(e => e.ExamId)
                    .HasName("PK_Exam");

                entity.ToTable("pExam");

                entity.Property(e => e.EndDate).HasColumnType("date");

                entity.Property(e => e.ExamDescription)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.ExamIdentifier1)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ExamIdentifier2)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ExameName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.StartDate).HasColumnType("date");
            });

            modelBuilder.Entity<PExamStudent>(entity =>
            {
                entity.HasKey(e => e.ExamStudentId)
                    .HasName("PK_pExamStudent_1");

                entity.ToTable("pExamStudent");

                entity.HasOne(d => d.Exam)
                    .WithMany(p => p.PExamStudent)
                    .HasForeignKey(d => d.ExamId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_pExamStudent_pExam");

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.PExamStudent)
                    .HasForeignKey(d => d.StudentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_pExamStudent_pStudent");
            });

            modelBuilder.Entity<PExamStudentProctoringStatus>(entity =>
            {
                entity.ToTable("pExamStudentProctoringStatus");

                entity.Property(e => e.EndedAt).HasColumnType("datetime");

                entity.Property(e => e.SessionId)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.StartedAt).HasColumnType("datetime");

                entity.Property(e => e.Uniqueidentifier)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<PExamStudentVerification>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.ExamId, e.StudentId, e.StepId });

                entity.ToTable("pExamStudentVerification");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Ipaddress)
                    .HasColumnName("IPAddress")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MachineName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Timestamp).HasColumnType("date");
            });

            modelBuilder.Entity<PProctorComments>(entity =>
            {
                entity.ToTable("pProctorComments");

                entity.Property(e => e.Comment)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Datetime).HasColumnType("datetime");
            });

            modelBuilder.Entity<PProctoringEnum>(entity =>
            {
                entity.ToTable("pProctoringEnum");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ProctoringStatus)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<PProctroingEvaluation>(entity =>
            {
                entity.ToTable("pProctroingEvaluation");

                entity.Property(e => e.Comments)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateBy)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<PStudent>(entity =>
            {
                entity.ToTable("pStudent");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DoB).HasColumnType("date");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MiddleName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.StudentIdentity)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.StudentUniqueId)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<PStudentExamPicture>(entity =>
            {
                entity.HasKey(e => e.StudentPictureId)
                    .HasName("PK_pStudentEpStudentExamPicture");

                entity.ToTable("pStudentExamPicture");

                entity.Property(e => e.Picture).IsRequired();

                entity.Property(e => e.Timestamp).HasColumnType("datetime");
            });

            modelBuilder.Entity<PStudentFaculty>(entity =>
            {
                entity.ToTable("pStudentFaculty");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450);
            });

            modelBuilder.Entity<ProctoringQueue>(entity =>
            {
                entity.Property(e => e.DateTimeInserted).HasColumnType("datetime");

                entity.Property(e => e.DateTimePicked).HasColumnType("datetime");

                entity.Property(e => e.DateTimeProcessed).HasColumnType("datetime");

                entity.Property(e => e.OrgCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Url)
                    .HasMaxLength(500)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
