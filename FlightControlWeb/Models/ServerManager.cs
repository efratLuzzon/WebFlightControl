﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class ServerManager : IServerManager
    {
        //private SqliteDB db = SqliteDB.Instance;
        private IDataManager db;
        public ServerManager(IDataManager db)
        {
            this.db = db;
        }
        public void AddServer(ServerFlight s)
        {
            if (s.ServerId == null || s.ServerUrl == null)
            {
                throw new Exception();
            }
            db.AddServer(s);
        }

        public void deleteServer(string id)
        {
            db.RemoveServer(id);
        }

        public List<ServerFlight> GetServerFlights()
        {
            return db.GetServers();
        }
    }
}
