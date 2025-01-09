using SWEN1_MCTG.Classes;

namespace SWEN1_MCTG.Data;

public interface IPackageService
{
    Task PurchasePackageAsync(int userId, GlobalEnums.PackageType packageType);
}