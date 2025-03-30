using System;
using Medicloud.Models;

namespace Medicloud.DAL.Repository.Kassa
{
    public interface IKassaRepo
    {
        List<KassaDAO> GetAllKassaListByOrganization(int organizationID);
        List<KassaDAO> GetUserKassaByOrganization(long organizationID, long userID);
        List<KassaDAO> GetUserAllowedKassaList(int userID);
        long CreateKassa(string name, long organizationID);
        int InsertKassaToUser(int userID, long kassaID, bool read_only, bool full_access);
        int RemoveKassaFromUser(int userID, int kassaID);
    }
}

