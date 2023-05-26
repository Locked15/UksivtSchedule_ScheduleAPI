using Microsoft.EntityFrameworkCore;
using ScheduleAPI.Models.Entities.Tables;
using ScheduleAPI.Models.Entities.Views;

namespace ScheduleAPI.Models.Entities;

public partial class DataContext : DbContext
{
    public DataContext()
    {
    }

    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    public virtual DbSet<BasicSchedule> BasicSchedules { get; set; } = null!;

    public virtual DbSet<FinalSchedule> FinalSchedules { get; set; } = null!;

    public virtual DbSet<Lesson> Lessons { get; set; } = null!;

    public virtual DbSet<ScheduleReplacement> ScheduleReplacements { get; set; } = null!;

    public virtual DbSet<TargetCycle> TargetCycles { get; set; } = null!;

    public virtual DbSet<Teacher> Teachers { get; set; } = null!;

    public virtual DbSet<UtilityAtomicDate> UtilityAtomicDates { get; set; } = null!;

    public virtual DbSet<UtilityLessonGroup> UtilityLessonGroups { get; set; } = null!;

    public virtual DbSet<UtilityLessonTeacher> UtilityLessonTeachers { get; set; } = null!;

    public virtual DbSet<UtilityPassedHour> UtilityPassedHours { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Russian_Russia.1251");

        modelBuilder.Entity<BasicSchedule>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("basic_schedule_pkey");

            entity.ToTable("basic_schedule");

            entity.HasIndex(e => e.CommitHash, "basic_schedule_commit_hash_key").IsUnique();

            entity.HasIndex(e => e.TargetGroup, "basic_schedule_group_index");

            entity.HasIndex(e => new { e.TargetGroup, e.CycleId, e.DayIndex }, "basic_schedule_target_group_cycle_id_day_index_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CommitHash).HasColumnName("commit_hash");
            entity.Property(e => e.CycleId).HasColumnName("cycle_id");
            entity.Property(e => e.DayIndex).HasColumnName("day_index");
            entity.Property(e => e.TargetGroup).HasColumnName("target_group");

            entity.HasOne(d => d.Cycle).WithMany(p => p.BasicSchedules)
                .HasForeignKey(d => d.CycleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("basic_schedule_cycle_id_fkey");
        });

        modelBuilder.Entity<FinalSchedule>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("schedule_pkey");

            entity.ToTable("final_schedule");

            entity.HasIndex(e => e.TargetGroup, "idx_final_schedule_target");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('schedule_id_seq'::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.CommitHash).HasColumnName("commit_hash");
            entity.Property(e => e.CycleId).HasColumnName("cycle_id");
            entity.Property(e => e.ScheduleDate).HasColumnName("schedule_date");
            entity.Property(e => e.TargetGroup).HasColumnName("target_group");

            entity.HasOne(d => d.Cycle).WithMany(p => p.FinalSchedules)
                .HasForeignKey(d => d.CycleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("final_schedule_cycle_id_fkey");
        });

        modelBuilder.Entity<Lesson>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("lesson_pkey");

            entity.ToTable("lesson");

            entity.HasIndex(e => e.BasicId, "idx_lesson_basic_id");

            entity.HasIndex(e => e.BasicId, "idx_lesson_final_id");

            entity.HasIndex(e => e.ReplacementId, "idx_lesson_replacement_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BasicId).HasColumnName("basic_id");
            entity.Property(e => e.FinalId).HasColumnName("final_id");
            entity.Property(e => e.IsChanged).HasColumnName("is_changed");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Number).HasColumnName("number");
            entity.Property(e => e.Place).HasColumnName("place");
            entity.Property(e => e.ReplacementId).HasColumnName("replacement_id");
            entity.Property(e => e.TeacherId).HasColumnName("teacher_id");

            entity.HasOne(d => d.Basic).WithMany(p => p.Lessons)
                .HasForeignKey(d => d.BasicId)
                .HasConstraintName("lesson_basic_id_fkey");

            entity.HasOne(d => d.Final).WithMany(p => p.Lessons)
                .HasForeignKey(d => d.FinalId)
                .HasConstraintName("lesson_final_id_fkey");

            entity.HasOne(d => d.Replacement).WithMany(p => p.Lessons)
                .HasForeignKey(d => d.ReplacementId)
                .HasConstraintName("lesson_replacement_id_fkey");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Lessons)
                .HasForeignKey(d => d.TeacherId)
                .HasConstraintName("lesson_teacher_id_fkey");
        });

        modelBuilder.Entity<ScheduleReplacement>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("replacement_pkey");

            entity.ToTable("schedule_replacement");

            entity.HasIndex(e => e.TargetGroup, "idx_schedule_replacement_target");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('replacement_id_seq'::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.CommitHash).HasColumnName("commit_hash");
            entity.Property(e => e.CycleId).HasColumnName("cycle_id");
            entity.Property(e => e.IsAbsolute)
                .HasDefaultValueSql("false")
                .HasColumnName("is_absolute");
            entity.Property(e => e.ReplacementDate).HasColumnName("replacement_date");
            entity.Property(e => e.TargetGroup).HasColumnName("target_group");

            entity.HasOne(d => d.Cycle).WithMany(p => p.ScheduleReplacements)
                .HasForeignKey(d => d.CycleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("schedule_replacement_cycle_id_fkey");
        });

        modelBuilder.Entity<TargetCycle>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("target_date_pkey");

            entity.ToTable("target_cycle");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('target_date_id_seq'::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.Semester).HasColumnName("semester");
            entity.Property(e => e.Year).HasColumnName("year");
        });

        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("teacher_pkey");

            entity.ToTable("teacher");

            entity.HasIndex(e => new { e.Surname, e.Name, e.Patronymic }, "teacher_data_must_be_unique").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Patronymic).HasColumnName("patronymic");
            entity.Property(e => e.Surname).HasColumnName("surname");
        });

        modelBuilder.Entity<UtilityAtomicDate>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("utility_atomic_date");

            entity.Property(e => e.DayOfMonth).HasColumnName("day_of_month");
            entity.Property(e => e.DayOfWeek).HasColumnName("day_of_week");
            entity.Property(e => e.Month).HasColumnName("month");
            entity.Property(e => e.Year).HasColumnName("year");
        });

        modelBuilder.Entity<UtilityLessonGroup>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("utility_lesson_group");

            entity.Property(e => e.LessonHoursPassed).HasColumnName("lesson_hours_passed");
            entity.Property(e => e.LessonIsChanged).HasColumnName("lesson_is_changed");
            entity.Property(e => e.LessonName).HasColumnName("lesson_name");
            entity.Property(e => e.LessonNumber).HasColumnName("lesson_number");
            entity.Property(e => e.LessonPlace).HasColumnName("lesson_place");
            entity.Property(e => e.LessonTeacher).HasColumnName("lesson_teacher");
        });

        modelBuilder.Entity<UtilityLessonTeacher>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("utility_lesson_teacher");

            entity.Property(e => e.LessonGroup).HasColumnName("lesson_group");
            entity.Property(e => e.LessonHoursPassed).HasColumnName("lesson_hours_passed");
            entity.Property(e => e.LessonIsChanged).HasColumnName("lesson_is_changed");
            entity.Property(e => e.LessonName).HasColumnName("lesson_name");
            entity.Property(e => e.LessonNumber).HasColumnName("lesson_number");
            entity.Property(e => e.LessonPlace).HasColumnName("lesson_place");
        });

        modelBuilder.Entity<UtilityPassedHour>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("utility_passed_hours");

            entity.Property(e => e.HoursPassed).HasColumnName("hours_passed");
            entity.Property(e => e.LessonName).HasColumnName("lesson_name");
            entity.Property(e => e.TargetCycle).HasColumnName("target_cycle");
            entity.Property(e => e.TargetGroup).HasColumnName("target_group");
        });

        modelBuilder.HasSequence<int>("lesson_id_seq");
        modelBuilder.HasSequence<int>("replacement_id_seq");
        modelBuilder.HasSequence<int>("schedule_id_seq");
        modelBuilder.HasSequence<int>("target_date_id_seq");
        modelBuilder.HasSequence<int>("teacher_id_seq");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
