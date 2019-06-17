﻿// <auto-generated />
using System;
using Everest.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Everest.Identity.Migrations
{
    [DbContext(typeof(PersistenceContext))]
    [Migration("20190617071119_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.11-servicing-32099")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Everest.Identity.Models.Account", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("APIURL");

                    b.Property<string>("AboutMe");

                    b.Property<DateTime>("BirthDate");

                    b.Property<string>("City");

                    b.Property<string>("Country");

                    b.Property<string>("Email");

                    b.Property<string>("Gender");

                    b.Property<string>("ImageName");

                    b.Property<string>("ImageURL");

                    b.Property<string>("Name");

                    b.Property<string>("NationalId");

                    b.Property<string>("Password");

                    b.Property<string>("PhoneNumber");

                    b.Property<string>("PostalCode");

                    b.Property<DateTime>("RegistrationDate");

                    b.Property<string>("ResetPasswordCode");

                    b.Property<DateTime>("ResetPasswordCodeCreateTime");

                    b.Property<string>("State");

                    b.Property<string>("Street");

                    b.Property<string>("Surname");

                    b.Property<string>("Username");

                    b.Property<string>("WebURL");

                    b.Property<string>("Website");

                    b.HasKey("Id");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("Everest.Identity.Models.Authorization", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AccessToken");

                    b.Property<string>("ClientId");

                    b.Property<string>("ConnectionId");

                    b.Property<long?>("ConnectionId1");

                    b.Property<string>("RefreshToken");

                    b.Property<DateTime>("RegistrationDate");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.HasIndex("ConnectionId1");

                    b.ToTable("Authorizations");
                });

            modelBuilder.Entity("Everest.Identity.Models.Client", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ImageName");

                    b.Property<string>("ImageURL");

                    b.Property<string>("Name");

                    b.Property<string>("RedirectURL");

                    b.Property<DateTime>("RegistrationDate");

                    b.Property<string>("SecretCode");

                    b.HasKey("Id");

                    b.ToTable("Clients");
                });

            modelBuilder.Entity("Everest.Identity.Models.Connection", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AccountId");

                    b.Property<DateTime?>("BeginDate");

                    b.Property<DateTime?>("EndDate");

                    b.Property<bool>("IsPersistent");

                    b.Property<string>("Navigator");

                    b.Property<string>("OS");

                    b.Property<DateTime>("RegistrationDate");

                    b.Property<string>("RemoteAddress");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.ToTable("Connections");
                });

            modelBuilder.Entity("Everest.Identity.Models.Authorization", b =>
                {
                    b.HasOne("Everest.Identity.Models.Client", "Client")
                        .WithMany("Authorizations")
                        .HasForeignKey("ClientId");

                    b.HasOne("Everest.Identity.Models.Connection", "Connection")
                        .WithMany("Authorizations")
                        .HasForeignKey("ConnectionId1");
                });

            modelBuilder.Entity("Everest.Identity.Models.Connection", b =>
                {
                    b.HasOne("Everest.Identity.Models.Account", "Account")
                        .WithMany("Connections")
                        .HasForeignKey("AccountId");
                });
#pragma warning restore 612, 618
        }
    }
}
