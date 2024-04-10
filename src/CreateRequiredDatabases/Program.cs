using Marketing.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Sales.Data;
using Shipping.Data;
using Warehouse.Data;

/*
 * Entity Framework .Database.EnsureCreated() works only if the database is empty
 * in this demo NServiceBus persistence might populate the database _before_ EF
 * checks for te first time. If that happens EF never creates the db. Thus, we create
 * it while building the solution. Another option (better, but far more complex
 * for this demo) would have been to use migrations.
 */

void CreateRequiredTables(DbContext context)
{
    var databaseCreator = (RelationalDatabaseCreator)context.Database.GetService<IDatabaseCreator>();

    if (databaseCreator.HasTables())
    {
        return;
    }
    
    databaseCreator.CreateTables();
}

CreateRequiredTables(new SalesContext());
CreateRequiredTables(new MarketingContext());
CreateRequiredTables(new ShippingContext());
CreateRequiredTables(new WarehouseContext());