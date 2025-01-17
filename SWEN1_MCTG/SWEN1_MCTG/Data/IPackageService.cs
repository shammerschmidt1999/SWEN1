using SWEN1_MCTG.Classes;
using static SWEN1_MCTG.GlobalEnums;

namespace SWEN1_MCTG.Data;

public interface IPackageService
{
    Task PurchasePackageAsync(int userId, PackageType packageType);
}