using Microsoft.EntityFrameworkCore;
using SupplierGuard.Domain.Entities;
using SupplierGuard.Domain.Enums;

namespace SupplierGuard.Infrastructure.Persistence.Seeds
{
    /// <summary>
    /// Class to populate the database with initial test data.
    /// </summary>
    public static class ApplicationDbContextSeed
    {
        /// <summary>
        /// Seeds initial data in the database.
        /// </summary>
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            // Check if data already exists
            if (await context.Suppliers.AnyAsync())
            {
                Console.WriteLine("Database already contains data. Skipping seed.");
                return;
            }

            Console.WriteLine("Seeding database with initial data...");

            var suppliers = GetSeedSuppliers();

            await context.Suppliers.AddRangeAsync(suppliers);
            await context.SaveChangesAsync();

            Console.WriteLine($"Successfully seeded {suppliers.Count} suppliers.");
        }

        /// <summary>
        /// Gets the list of suppliers for initial seed.
        /// </summary>
        private static List<Supplier> GetSeedSuppliers()
        {
            return new List<Supplier>
        {
            // Supplier 1: Large company from Peru
            Supplier.Create(
                legalName: "Acme Corporation S.A.",
                commercialName: "ACME Corp",
                taxId: "20123456789",
                phoneNumber: "+51987654321",
                email: "contacto@acmecorp.com.pe",
                website: "https://www.acmecorp.com.pe",
                physicalAddress: "Av. Javier Prado Este 5250, La Molina, Lima",
                country: Country.Peru,
                annualRevenue: 15000000.00m
            ),

            // Supplier 2: Tech company from Chile
            Supplier.Create(
                legalName: "Tech Solutions Limitada",
                commercialName: "TechSol",
                taxId: "20987654321",
                phoneNumber: "+56912345678",
                email: "info@techsol.cl",
                website: "https://www.techsolutions.cl",
                physicalAddress: "Av. Apoquindo 4800, Las Condes, Santiago",
                country: Country.Chile,
                annualRevenue: 8500000.00m
            ),

            // Supplier 3: Importer from Colombia
            Supplier.Create(
                legalName: "Global Imports S.A.S.",
                commercialName: "Global Imports",
                taxId: "20555666777",
                phoneNumber: "+573001234567",
                email: "ventas@globalimports.com.co",
                website: "https://www.globalimports.com.co",
                physicalAddress: "Carrera 7 No. 71-21, Bogotá",
                country: Country.Colombia,
                annualRevenue: 12000000.00m
            ),

            // Supplier 4: Construction company from Brazil
            Supplier.Create(
                legalName: "Construtora Brasil Ltda.",
                commercialName: "ConstraBrasil",
                taxId: "20111222333",
                phoneNumber: "+5511987654321",
                email: "contato@constrabrasil.com.br",
                website: "https://www.constrabrasil.com.br",
                physicalAddress: "Av. Paulista 1578, São Paulo",
                country: Country.Brazil,
                annualRevenue: 25000000.00m
            ),

            // Supplier 5: Distributor from Argentina
            Supplier.Create(
                legalName: "Distribuidora del Sur S.R.L.",
                commercialName: "Distri Sur",
                taxId: "20444555666",
                phoneNumber: "+541145678901",
                email: "info@distrisur.com.ar",
                website: "https://www.distrisur.com.ar",
                physicalAddress: "Av. Corrientes 1234, Buenos Aires",
                country: Country.Argentina,
                annualRevenue: 7500000.00m
            ),

            // Supplier 6: Small company from Mexico
            Supplier.Create(
                legalName: "Servicios Integrales del Norte S.A. de C.V.",
                commercialName: "ServNorte",
                taxId: "20777888999",
                phoneNumber: "+525512345678",
                email: "contacto@servnorte.mx",
                website: null, // No website
                physicalAddress: "Av. Insurgentes Sur 1458, Ciudad de México",
                country: Country.Mexico,
                annualRevenue: 3200000.00m
            ),

            // Supplier 7: Tech startup from United States
            Supplier.Create(
                legalName: "InnovateTech Inc.",
                commercialName: "InnovateTech",
                taxId: "20321654987",
                phoneNumber: "+14155551234",
                email: "hello@innovatetech.io",
                website: "https://www.innovatetech.io",
                physicalAddress: "123 Market Street, San Francisco, CA 94103",
                country: Country.UnitedStates,
                annualRevenue: 5000000.00m
            ),

            // Supplier 8: Medium company from Spain
            Supplier.Create(
                legalName: "Suministros Ibéricos S.L.",
                commercialName: "SumIber",
                taxId: "20159753864",
                phoneNumber: "+34912345678",
                email: "ventas@sumiber.es",
                website: "https://www.suministrosibericos.es",
                physicalAddress: "Calle Gran Vía 28, Madrid",
                country: Country.Spain,
                annualRevenue: 9800000.00m
            ),

            // Supplier 9: Logistics company from Germany
            Supplier.Create(
                legalName: "Deutsche Logistik GmbH",
                commercialName: "DeutLog",
                taxId: "20852963741",
                phoneNumber: "+4930123456789",
                email: "info@deutschelogistik.de",
                website: "https://www.deutschelogistik.de",
                physicalAddress: "Friedrichstraße 95, Berlin",
                country: Country.Germany,
                annualRevenue: 18000000.00m
            ),

            // Supplier 10: Consulting company from Canada
            Supplier.Create(
                legalName: "Maple Consulting Corp.",
                commercialName: "MapleConsult",
                taxId: "20753951846",
                phoneNumber: "+14165551234",
                email: "contact@mapleconsulting.ca",
                website: "https://www.mapleconsulting.ca",
                physicalAddress: "100 King Street West, Toronto, ON M5X 1A9",
                country: Country.Canada,
                annualRevenue: 6500000.00m
            ),

            // Supplier 11: Company from Ecuador (low revenue)
            Supplier.Create(
                legalName: "Comercial Andina Cía. Ltda.",
                commercialName: "Comercial Andina",
                taxId: "20147258369",
                phoneNumber: "+593987654321",
                email: "ventas@comercialandina.ec",
                website: null,
                physicalAddress: "Av. 10 de Agosto N39-61, Quito",
                country: Country.Ecuador,
                annualRevenue: 850000.00m
            ),

            // Supplier 12: Company from Uruguay (very high revenue)
            Supplier.Create(
                legalName: "Mega Distribuciones S.A.",
                commercialName: "MegaDist",
                taxId: "20369258147",
                phoneNumber: "+59899123456",
                email: "info@megadist.com.uy",
                website: "https://www.megadistribuciones.com.uy",
                physicalAddress: "Av. 18 de Julio 1234, Montevideo",
                country: Country.Uruguay,
                annualRevenue: 45000000.00m
            )
        };
        }

        /// <summary>
        /// Clears all data from the database.
        /// USE ONLY IN DEVELOPMENT.
        /// </summary>
        public static async Task ClearDatabaseAsync(ApplicationDbContext context)
        {
            Console.WriteLine("WARNING: Clearing all data from database...");

            context.Suppliers.RemoveRange(context.Suppliers);
            await context.SaveChangesAsync();

            Console.WriteLine("Database cleared successfully.");
        }

        /// <summary>
        /// Re-seeds the database (clears and repopulates).
        /// USE ONLY IN DEVELOPMENT.
        /// </summary>
        public static async Task ReseedDatabaseAsync(ApplicationDbContext context)
        {
            await ClearDatabaseAsync(context);
            await SeedAsync(context);
        }
    }
}
