#nullable disable
using DashLib.Models;
using DashLib.Models.Auth;
using DashLib.Models.Dashboard;
using DashLib.Models.Monitoring;
using DashLib.Models.MonitoringTargetContainers;
using DashLib.Models.Network;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace dankweb.API;
public partial class DashDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public DashDbContext()
    {
        Database.EnsureCreated();
    }

    public DashDbContext(DbContextOptions<DashDbContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }

    public virtual DbSet<ShortcutItem> ShortcutItems { get; set; }
    public virtual DbSet<DirectoryItem> DirectoryItems { get; set; }
    public virtual DbSet<ClockWidget> ClockWidgets { get; set; }
    public virtual DbSet<DeviceStatusWidget> DeviceStatusWidgets { get; set; }
    public virtual DbSet<IpMonitoringTarget> IpTargets { get; set; }
    public virtual DbSet<DnsMonitoringTarget> DnsTargets { get; set; }
    public virtual DbSet<SubnetContainer> SubnetContainers { get; set; }
    public virtual DbSet<DnsContainer> DnsContainers { get; set; }
    public virtual DbSet<PortState> PortStates { get; set; }
    public virtual DbSet<PingState> PingStates { get; set; }
    public virtual DbSet<LogEntry> LogEntries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<BaseMonitoringTarget>(entity =>
        {
            entity.ToTable("monitoring_targets");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.UseTptMappingStrategy();

            entity.Ignore(e => e.Parent);

            entity.HasMany(e => e.TcpMonitorStates)
                .WithOne(e => e.Target)
                .HasForeignKey(e => e.TargetId)
                .HasPrincipalKey(e => e.Id);

            entity.HasMany(e => e.IcmpMonitorStates)
                .WithOne(e => e.Target)
                .HasForeignKey(e => e.TargetId)
                .HasPrincipalKey(e => e.Id);

            entity.HasIndex(e => e.Id);

            entity.Property(e => e.Hostname)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.Property(e => e.IsMonitoredIcmp).IsRequired();
            entity.Property(e => e.IsMonitoredTcp).IsRequired();
            entity.Property(e => e.TcpPortsMonitored).IsRequired();
        });

        modelBuilder.Entity<IpMonitoringTarget>(entity =>
        {
            entity.ToTable("ips");

            entity.HasIndex(e => e.Address).IsUnique();
            entity.Property(e => e.Address)
                .IsRequired()
                .HasMaxLength(4)
                .IsFixedLength();

            entity.Property(e => e.ParentId).IsRequired();
            entity.HasOne<SubnetContainer>("Parent")
                .WithMany(e => e.Children)
                .HasForeignKey("ParentId");
        });

        modelBuilder.Entity<DnsMonitoringTarget>(entity =>
        {
            entity.ToTable("dns");

            entity.HasIndex(e => e.Address).IsUnique();
            entity.Property(e => e.Address)
                .IsRequired()
                .HasMaxLength(512);
            entity.Property(e => e.ParentId).IsRequired();

            entity.Property(e => e.ParentId).IsRequired();
            entity.HasOne<DnsContainer>("Parent")
                .WithMany(e => e.Children)
                .HasForeignKey("ParentId");
        });

        modelBuilder.Entity<ClockWidget>(entity =>
        {
            entity.ToTable("clock_widgets");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id);
            entity.Property(e => e.DisplayName)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Description)
                .HasMaxLength(300)
                .IsUnicode(false);
        });

        modelBuilder.Entity<DeviceStatusWidget>(entity =>
        {
            entity.ToTable("device_status_widgets");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id);
            entity.Property(e => e.DisplayName)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Description)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.IP_ID);
            entity.HasOne(e => e.IP).WithOne(e => e.DeviceStatusWidget).HasForeignKey<DeviceStatusWidget>(e => e.IP_ID);
        });

        modelBuilder.Entity<ShortcutItem>(entity =>
        {
            entity.ToTable("shortcut_items");

            entity.Property(e => e.Id);
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Description)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.DisplayName)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Icon).IsUnicode(false);
            entity.Property(e => e.Url)
                .IsRequired()
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.HasOne(e => e.Parent).WithMany(e => e.Children).HasForeignKey(e => e.ParentId);
        });

        modelBuilder.Entity<DirectoryItem>(entity =>
        {
            entity.ToTable("directory_items");

            entity.Property(e => e.Id);
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Description)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.DisplayName)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Icon)
                .IsUnicode(false);
            entity.HasMany<ShortcutItem>(e => e.Children).WithOne(e => e.Parent);
        });

        modelBuilder.Entity<SubnetContainer>(entity =>
        {
            entity.ToTable("subnet_containers");

            entity.HasIndex(e => e.Id);

            entity.Property(e => e.Id);
            entity.Property(e => e.EndAddress)
                .IsRequired()
                .HasMaxLength(4)
                .IsFixedLength();
            entity.Property(e => e.StartAddress)
                .IsRequired()
                .HasMaxLength(4)
                .IsFixedLength();
            entity.Property(e => e.Address)
                .IsRequired()
                .HasMaxLength(4)
                .IsFixedLength();
            entity.Property(e => e.SubnetMask)
                .IsRequired()
                .HasMaxLength(4)
                .IsFixedLength();
        });

        modelBuilder.Entity<DnsContainer>(entity =>
        {
            entity.ToTable("dns_containers");

            entity.HasIndex(e => e.Id);
            entity.Property(e => e.Id);
            entity.Property(e => e.DisplayName).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<PortState>(entity =>
        {
            entity.ToTable("monitoring_ports");

            entity.HasIndex(e => e.Id);
            entity.Property(e => e.Id);
            entity.Property(e => e.TargetId).IsRequired();
            entity.Property(e => e.Timestamp).IsRequired();
            entity.Property(e => e.TargetPort).IsRequired().HasMaxLength(8);
            entity.Property(e => e.Response).IsRequired();
        });

        modelBuilder.Entity<PingState>(entity =>
        {
            entity.ToTable("monitoring_ping");

            entity.HasIndex(e => e.Id);
            entity.Property(e => e.Id);
            entity.Property(e => e.TargetId).IsRequired();
            entity.Property(e => e.Timestamp).IsRequired();
            entity.Property(e => e.Response).IsRequired();
        });

        modelBuilder.Entity<LogEntry>(entity =>
        {
            entity.ToTable("logs");
            entity.HasIndex(e => e.Id);
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Timestamp).HasColumnType("DateTime");
            entity.Property(e => e.Level).IsRequired();
            entity.Property(e => e.Message).IsRequired();
            entity.Property(e => e.Source).IsRequired();
        });

    }
}