using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Medicloud.DAL.Infrastructure.Abstract;
using Medicloud.Models;
using MySql.Data.MySqlClient;

namespace Medicloud.DAL.Repository.Kassa
{
    public class KassaRepo:IKassaRepo
    {
        private readonly IUnitOfWork _unitOfWork;

        public KassaRepo(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<KassaDAO> GetAllKassaListByOrganization(int organizationID)
        {
            List<KassaDAO> kassaList = new List<KassaDAO>();

            try
            {
                _unitOfWork.BeginConnection();
                var con = _unitOfWork.GetConnection();

                string sql = "SELECT id, name FROM kassa WHERE organizationID = @organizationID;";
                kassaList = con.Query<KassaDAO>(sql, new { organizationID }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return kassaList;
        }

        public List<KassaDAO> GetUserKassaByOrganization(long organizationID, long userID)
        {
            List<KassaDAO> kassaList = new List<KassaDAO>();

            try
            {
                _unitOfWork.BeginConnection();
                var con = _unitOfWork.GetConnection();

                string sql = @"
                    SELECT a.id, a.kassaID, a.read_only, a.full_access, 
                           (SELECT name FROM kassa WHERE id = a.kassaID) AS name
                    FROM kassa_user_rel a
                    WHERE a.kassaID IN (
                        SELECT id FROM kassa WHERE organizationID = @organizationID
                    ) AND a.userID = @userID;
                ";

                kassaList = con.Query<KassaDAO>(sql, new { organizationID, userID }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return kassaList;
        }

        public List<KassaDAO> GetUserAllowedKassaList(int userID)
        {
            List<KassaDAO> kassaList = new List<KassaDAO>();

            try
            {
                _unitOfWork.BeginConnection();
                var con = _unitOfWork.GetConnection();

                string sql = @"
                    SELECT a.id, a.kassaID, 
                           (SELECT name FROM kassa WHERE id = a.kassaID) AS name
                    FROM kassa_user_rel a
                    WHERE a.userID = @userID;
                ";

                kassaList = con.Query<KassaDAO>(sql, new { userID }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return kassaList;
        }

        public long CreateKassa(string name, long organizationID)
        {
            long lastID = 0;

            try
            {
                _unitOfWork.BeginConnection();
                var con = _unitOfWork.GetConnection();

                string sql = "INSERT INTO kassa (name, organizationID) VALUES (@name, @organizationID); SELECT LAST_INSERT_ID();";
                lastID = con.ExecuteScalar<long>(sql, new { name, organizationID });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return lastID;
        }

        public int InsertKassaToUser(int userID, long kassaID, bool read_only, bool full_access)
        {
            int result = 0;

            try
            {
                if (userID > 0 && kassaID > 0)
                {
                    _unitOfWork.BeginConnection();
                    var con = _unitOfWork.GetConnection();

                    string sql = @"
                        INSERT INTO kassa_user_rel (kassaID, userID, read_only, full_access)
                        SELECT @kassaID, @userID, @read_only, @full_access FROM DUAL
                        WHERE NOT EXISTS (
                            SELECT 1 FROM kassa_user_rel WHERE kassaID = @kassaID AND userID = @userID
                        );
                    ";

                    result = con.Execute(sql, new { kassaID, userID, read_only, full_access });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return result;
        }

        public int RemoveKassaFromUser(int userID, int kassaID)
        {
            int result = 0;

            try
            {
                _unitOfWork.BeginConnection();
                var con = _unitOfWork.GetConnection();

                string sql = "DELETE FROM kassa_user_rel WHERE userID = @userID AND kassaID = @kassaID;";
                result = con.Execute(sql, new { userID, kassaID });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return result;
        }
    }
}
