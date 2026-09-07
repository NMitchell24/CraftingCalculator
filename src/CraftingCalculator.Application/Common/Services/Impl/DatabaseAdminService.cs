using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Application.Common.Interfaces.DAO;

namespace CraftingCalculator.Application.Common.Services.Impl;

public class DatabaseAdminService(IDatabaseAdminDAO dao) : IDatabaseAdminService
{
    public Task DeleteAllDataAsync() => dao.DeleteAllDataAsync();
}
