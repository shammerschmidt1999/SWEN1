using SWEN1_MCTG.Classes;

namespace SWEN1_MCTG.Data;

public interface IPackageService
{
    void PurchasePackage(int userId, GlobalEnums.PackageType packageType);
}