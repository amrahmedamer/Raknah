using System.Reflection;

namespace Raknah.Persistence;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<ParkingSpot> ParkingSpots { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        //modelBuilder.Entity<Reservation>().Property(r => r.Duration).
        //    HasComputedColumnSql("DATEDIFF(MINUTE, StartTimeOfParking, EndTimeOfParking)");

        modelBuilder.Entity<ParkingSpot>().HasData(
      new ParkingSpot { Id = 1, SensorStatus = SensorStatus.Availiable, SensorCode = "Sensor-1", Name = "Spot-1", SpotStatus = SpotStatus.Availiable },
           new ParkingSpot { Id = 2, SensorStatus = SensorStatus.Availiable, SensorCode = "Sensor-2", Name = "Spot-2", SpotStatus = SpotStatus.Availiable },
           new ParkingSpot { Id = 3, SensorStatus = SensorStatus.Availiable, SensorCode = "Sensor-3", Name = "Spot-3", SpotStatus = SpotStatus.Availiable },
           new ParkingSpot { Id = 4, SensorStatus = SensorStatus.Availiable, SensorCode = "Sensor-4", Name = "Spot-4", SpotStatus = SpotStatus.Availiable }
       );

    }


}
