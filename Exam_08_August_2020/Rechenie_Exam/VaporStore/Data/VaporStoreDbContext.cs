﻿namespace VaporStore.Data
{
	using Microsoft.EntityFrameworkCore;
    using VaporStore.Data.Models;

    public class VaporStoreDbContext : DbContext
	{
		public VaporStoreDbContext()
		{
		}

		public VaporStoreDbContext(DbContextOptions options)
			: base(options)
		{
		}

		protected override void OnConfiguring(DbContextOptionsBuilder options)
		{
			if (!options.IsConfigured)
			{
				options
					.UseSqlServer(Configuration.ConnectionString);
			}
		}

		public DbSet<Tag> Tags { get; set; }
		public DbSet<Developer> Developers { get; set; }

		public DbSet<Genre> Genres { get; set; }

		public DbSet<User> Users { get; set; }
		public DbSet<Card> Cards{ get; set; }
		public DbSet<Game> Games { get; set; }
		public DbSet<Purchase> Purchases { get; set; }
		public DbSet<GameTag> GameTags { get; set; }



		protected override void OnModelCreating(ModelBuilder model)
		{
			model
			   .Entity<GameTag>(entity =>
			   {
				   entity.HasKey(et =>
				   new { et.GameId, et.TagId });
			   });

			model
			   .Entity<Game>(entity =>
			   {
				   entity.HasKey(et =>
				   new { et.DeveloperId, et.GenreId });
			   });


			model
			   .Entity<Purchase>(entity =>
			   {
				   entity.HasKey(et =>
				   new { et.CardId, et.GameId });
			   });

		}
	}
}