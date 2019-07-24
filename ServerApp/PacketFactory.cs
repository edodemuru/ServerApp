using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApp
{
    class PacketFactory
    {

        private static PacketFactory instance = null;
        private String connectionString = "Server=localhost;Database=PacketDB;Uid=root;Pwd=";
        private int numEsp32;

        private PacketFactory()
        {
        }

        public static PacketFactory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PacketFactory();
                }
                return instance;
            }
        }

        public void InsertPacket(Packet packet)
        {

            try
            {
                MySqlConnection databaseConnection = new MySqlConnection(connectionString);
                databaseConnection.Open();
                String sqlInsert = "insert into `packets`(`Id`, `ssid`, `channel`, `rssi`, `source_mac`, `esp32_mac`, `timestamp`, `hash`) values (@Id,@SID,@Cha,@RS,@MACs,@MACe,@Time,@H)";
                MySqlCommand cmd = new MySqlCommand(sqlInsert, databaseConnection);
                cmd.Parameters.Add("@Id", MySqlDbType.Int32).Value = packet.Id;
                cmd.Parameters.Add("@Time", MySqlDbType.DateTime).Value = packet.Timestamp;
                cmd.Parameters.Add("@Cha", MySqlDbType.Int32).Value = packet.Channel;
                cmd.Parameters.Add("@SID", MySqlDbType.VarChar).Value = packet.Ssid;
                cmd.Parameters.Add("@RS", MySqlDbType.Int32).Value = packet.Rssi;
                cmd.Parameters.Add("@MACs", MySqlDbType.VarChar).Value = packet.MacSource;
                cmd.Parameters.Add("@MACe", MySqlDbType.VarChar).Value = packet.MacEsp32;
                cmd.Parameters.Add("@H", MySqlDbType.VarChar).Value = packet.Hash;
                int i = cmd.ExecuteNonQuery();
                databaseConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("error " + ex.Message);
                return;
            }
        }

        public int GetPacketMaxId()
        {
            int result = -1;
            try
            {
                MySqlConnection databaseConnection = new MySqlConnection(connectionString);
                databaseConnection.Open();
                String sqlInsert = "select max(id) from packets";
                MySqlCommand cmd = new MySqlCommand(sqlInsert, databaseConnection);
                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader != null && reader.HasRows && reader.Read())
                {
                    if (!reader.IsDBNull(0))
                        result = reader.GetInt32(0);

                }
                reader.Close();
                databaseConnection.Close();

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("error " + ex.Message);
                return -1;
            }
        }

        //Get list of hash of packets received from all esp32
        public List<String> GetListHashFiltered()
        {
            List<String> listHash = new List<String>();

            try
            {
                MySqlConnection databaseConnection = new MySqlConnection(connectionString);
                databaseConnection.Open();

                //Raggruppo inizialmente per hash e per esp32, così elimino i doppioni, poi faccio un ulteriore raggruppamento per hash, così da individuare i pacchetti ricevuti da entrambe le schede
                String sqlQuery = "select hash from (select* from packets group by hash,esp32_mac) as filteredPackets group by hash having count(*) = " + NumEsp32;

                MySqlCommand cmd = new MySqlCommand(sqlQuery, databaseConnection);
                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    //while cicle to read the data
                    while (reader.Read())
                    {
                        //each row from the data matched by the query
                        listHash.Add(reader.GetString(0));

                    }
                }
                else
                {
                    Console.WriteLine("No rows found.");
                }

                //int i = cmd.ExecuteNonQuery();
                databaseConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("error " + ex.Message);
                return null;
            }

            return listHash;
        }

        //Get list of hash of packets received from all esp32
        public List<String> GetListHashFiltered(int lastId)
        {
            List<String> listHash = new List<String>();

            try
            {
                MySqlConnection databaseConnection = new MySqlConnection(connectionString);
                databaseConnection.Open();

                //Raggruppo inizialmente per hash e per esp32, così elimino i doppioni, poi faccio un ulteriore raggruppamento per hash, così da individuare i pacchetti ricevuti da entrambe le schede
                String sqlQuery = "select hash from (select* from packets group by hash,esp32_mac) as filteredPackets where id>= " + lastId + " group by hash having count(*) = " + NumEsp32;

                MySqlCommand cmd = new MySqlCommand(sqlQuery, databaseConnection);
                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    //while cicle to read the data
                    while (reader.Read())
                    {
                        //each row from the data matched by the query
                        listHash.Add(reader.GetString(0));

                    }
                }
                else
                {
                    Console.WriteLine("No rows found.");
                }

                //int i = cmd.ExecuteNonQuery();
                databaseConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("error " + ex.Message);
                return null;
            }

            return listHash;
        }

        public List<String> GetListHashFilteredInInterval(DateTime start, DateTime end)
        {
            List<String> listHash = new List<String>();

            try
            {
                MySqlConnection databaseConnection = new MySqlConnection(connectionString);
                databaseConnection.Open();

                //Raggruppo inizialmente per hash e per esp32, così elimino i doppioni, poi faccio un ulteriore raggruppamento per hash, così da individuare i pacchetti ricevuti da entrambe le schede
                String sqlQuery = "select hash from (select* from packets where timestamp BETWEEN '" + start + "' AND '" + end + "' group by hash,esp32_mac) as filteredPackets group by hash having count(*) = " + NumEsp32;

                MySqlCommand cmd = new MySqlCommand(sqlQuery, databaseConnection);
                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    //while cicle to read the data
                    while (reader.Read())
                    {
                        //each row from the data matched by the query
                        listHash.Add(reader.GetString(0));

                    }
                }
                else
                {
                    Console.WriteLine("No rows found.");
                }

                //int i = cmd.ExecuteNonQuery();
                databaseConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("error " + ex.Message);
                return null;
            }

            return listHash;
        }



        public int GetCountFromPacket(Packet p)
        {
            int numb = 0;
            //To convert SQLdataReader into String 
            try
            {
                MySqlConnection databaseConnection = new MySqlConnection(connectionString);
                databaseConnection.Open();

                String sqlQuery = "SELECT COUNT(*) from packets WHERE hash ='" + p.Hash + "' GROUP by esp32_mac";


                MySqlCommand cmd = new MySqlCommand(sqlQuery, databaseConnection);
                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    //while cicle to read the data
                    while (reader.Read())
                    {
                        //each row from the data matched by the query
                        //string []  row ={ reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3) }; //depends by the number of the coloumn
                        numb = reader.GetInt32(0);

                    }

                }
                else
                {
                    Console.WriteLine("No rows found.");
                }

                //int i = cmd.ExecuteNonQuery();
                databaseConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("error " + ex.Message);
                return -1;
            }

            return numb;
        }

        //Obtain list of packets from hash, selecting the most recent
        public List<Packet> GetListPkFilteredFromHash(String hash)
        {
            List<Packet> listPackets = new List<Packet>();

            try
            {
                MySqlConnection databaseConnection = new MySqlConnection(connectionString);
                databaseConnection.Open();

                String sqlQuery = "select * from packets where id IN(select MAX(id) from packets where hash ='" + hash + "' GROUP by esp32_mac) order by `packets`.`esp32_mac` asc";

                MySqlCommand cmd = new MySqlCommand(sqlQuery, databaseConnection);
                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    //while cicle to read the data
                    while (reader.Read())
                    {
                        //each row from the data matched by the query
                        Packet tmp = new Packet();

                        tmp.Id = reader.GetInt32(0);
                        tmp.Ssid = reader.GetString(1);
                        tmp.Channel = reader.GetInt32(2);
                        tmp.Rssi = reader.GetInt32(3);
                        tmp.MacSource = reader.GetString(4);
                        tmp.MacEsp32 = reader.GetString(5);
                        tmp.Timestamp = reader.GetDateTime(6);
                        tmp.Hash = reader.GetString(7);

                        listPackets.Add(tmp);

                    }
                }
                else
                {
                    Console.WriteLine("No rows found.");
                }

                //int i = cmd.ExecuteNonQuery();
                databaseConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("error " + ex.Message);
                return null;
            }

            return listPackets;
        }

        //Obtain list of packets from hash, selecting the most recent
        public List<Packet> GetListPkFilteredFromHash(String hash, int lastId)
        {
            List<Packet> listPackets = new List<Packet>();

            try
            {
                MySqlConnection databaseConnection = new MySqlConnection(connectionString);
                databaseConnection.Open();

                String sqlQuery = "select * from packets where id IN(select MAX(id) from packets where hash ='" + hash + "' GROUP by esp32_mac) and id >= " + lastId + " order by `packets`.`esp32_mac` asc";

                MySqlCommand cmd = new MySqlCommand(sqlQuery, databaseConnection);
                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    //while cicle to read the data
                    while (reader.Read())
                    {
                        //each row from the data matched by the query
                        Packet tmp = new Packet();

                        tmp.Id = reader.GetInt32(0);
                        tmp.Ssid = reader.GetString(1);
                        tmp.Channel = reader.GetInt32(2);
                        tmp.Rssi = reader.GetInt32(3);
                        tmp.MacSource = reader.GetString(4);
                        tmp.MacEsp32 = reader.GetString(5);
                        tmp.Timestamp = reader.GetDateTime(6);
                        tmp.Hash = reader.GetString(7);

                        listPackets.Add(tmp);

                    }
                }
                else
                {
                    Console.WriteLine("No rows found.");
                }

                //int i = cmd.ExecuteNonQuery();
                databaseConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("error " + ex.Message);
                return null;
            }

            return listPackets;
        }

        //Obtain list of packets from hash in specific interval
        public List<Packet> GetListPkFilteredFromHashInInterval(String hash, DateTime start, DateTime end)
        {
            List<Packet> listPackets = new List<Packet>();

            try
            {
                MySqlConnection databaseConnection = new MySqlConnection(connectionString);
                databaseConnection.Open();

                String sqlQuery = "select * from packets where id IN(select MAX(id) from packets where hash ='" + hash + "' GROUP by esp32_mac) AND timestamp BETWEEN '" + start + "' AND '" + end + "' order by `packets`.`esp32_mac` asc";

                MySqlCommand cmd = new MySqlCommand(sqlQuery, databaseConnection);
                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    //while cicle to read the data
                    while (reader.Read())
                    {
                        //each row from the data matched by the query
                        Packet tmp = new Packet();

                        tmp.Id = reader.GetInt32(0);
                        tmp.Ssid = reader.GetString(1);
                        tmp.Channel = reader.GetInt32(2);
                        tmp.Rssi = reader.GetInt32(3);
                        tmp.MacSource = reader.GetString(4);
                        tmp.MacEsp32 = reader.GetString(5);
                        tmp.Timestamp = reader.GetDateTime(6);
                        tmp.Hash = reader.GetString(7);

                        listPackets.Add(tmp);

                    }
                }
                else
                {
                    Console.WriteLine("No rows found.");
                }

                //int i = cmd.ExecuteNonQuery();
                databaseConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("error " + ex.Message);
                return null;
            }

            return listPackets;
        }



        //Given time interval find number of packets in such interval
        public int GetPacketNumInInterval(DateTime start, DateTime end)
        {
            int numPackets = 0;
            try
            {
                MySqlConnection databaseConnection = new MySqlConnection(connectionString);
                databaseConnection.Open();
                string format = "yyyy-MM-dd HH:mm:ss";
                //Old query because I haven't considered packets received by all esp32
                //String sqlQuery = "select count(*) from(select * from packets where timestamp BETWEEN '" + start.ToString(format) + "' and '" + end.ToString(format) + "' group by source_mac) as numDevices";
                String sqlQuery = "select count(*) from(select * from(select * from packets where timestamp BETWEEN '" + start.ToString(format) + "' and '" + end.ToString(format) + "' group by hash, esp32_mac) as filteredPackets group by HASH HAVING count(*) = " + NumEsp32 + ") as filteredData";

                MySqlCommand cmd = new MySqlCommand(sqlQuery, databaseConnection);
                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    //while cicle to read the data
                    while (reader.Read())
                    {
                        numPackets = reader.GetInt32(0);

                    }
                }
                else
                {
                    Console.WriteLine("No rows found.");
                }

                //int i = cmd.ExecuteNonQuery();
                databaseConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("error " + ex.Message);
                return -1;
            }

            return numPackets;
        }

        public List<Packet> GetListPacketInIntervalFromSourceMac(DateTime start, DateTime end, string sourceMac)
        {
            List<Packet> listPackets = new List<Packet>();
            try
            {
                MySqlConnection databaseConnection = new MySqlConnection(connectionString);
                databaseConnection.Open();
                string format = "yyyy-MM-dd HH:mm:ss";

                String sqlQuery = "select * from (select * from (select* from packets where timestamp BETWEEN '" + start.ToString(format) + "' and '" + end.ToString(format) + "' group by hash,esp32_mac) as filteredPackets group by HASH HAVING count(*) = " + NumEsp32 + ") as filteredData where source_mac = '" + sourceMac + "' order by `filteredData`.`timestamp` ASC";
                MySqlCommand cmd = new MySqlCommand(sqlQuery, databaseConnection);
                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    //while cicle to read the data
                    while (reader.Read())
                    {
                        //each row from the data matched by the query
                        Packet tmp = new Packet();

                        tmp.Id = reader.GetInt32(0);
                        tmp.Ssid = reader.GetString(1);
                        tmp.Channel = reader.GetInt32(2);
                        tmp.Rssi = reader.GetInt32(3);
                        tmp.MacSource = reader.GetString(4);
                        tmp.MacEsp32 = reader.GetString(5);
                        tmp.Timestamp = reader.GetDateTime(6);
                        tmp.Hash = reader.GetString(7);

                        listPackets.Add(tmp);

                    }
                }
                else
                {
                    Console.WriteLine("No rows found.");
                }

                //int i = cmd.ExecuteNonQuery();
                databaseConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("error " + ex.Message);
                return null;
            }

            return listPackets;
        }

        public List<string> GetMostFrequentPackets(DateTime start , DateTime end, int NumResults)
        {
            List<string> packetsFound = new List<string>();
            try
            {
                MySqlConnection databaseConnection = new MySqlConnection(connectionString);
                databaseConnection.Open();
                string format = "yyyy-MM-dd HH:mm:ss";

                String sqlQuery = "select source_mac from (select * from (select* from packets where timestamp BETWEEN '" + start.ToString(format) + "' and '" + end.ToString(format) + "' group by hash,esp32_mac) as filteredPackets group by HASH HAVING count(*) = " + NumEsp32 + ") as filteredData GROUP by source_mac ORDER BY count(*) DESC LIMIT " + NumResults;
                MySqlCommand cmd = new MySqlCommand(sqlQuery, databaseConnection);
                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    //while cicle to read the data
                    while (reader.Read())
                    {
                        //each row from the data matched by the query
                        string tmp = reader.GetString(0);                    
                        packetsFound.Add(tmp);

                    }
                }
                else
                {
                    Console.WriteLine("No rows found.");
                }

                //int i = cmd.ExecuteNonQuery();
                databaseConnection.Close();
            }

            catch (Exception ex)
            {
                Console.WriteLine("error " + ex.Message);
                return null;
            }

            return packetsFound;

        }

        public void InsertCoordinate(List<Device> Devices)
        {
            List<string> packetsFound = new List<string>();
            try
            {
                MySqlConnection databaseConnection = new MySqlConnection(connectionString);
                databaseConnection.Open();

                foreach (Device d in Devices) { 
                String sqlQuery = "update packets set position_X = " + d.X + ", position_Y = " + d.Y + " where id = " + d.Id;
                MySqlCommand cmd = new MySqlCommand(sqlQuery, databaseConnection);
                int i = cmd.ExecuteNonQuery();

                    
            }

                //int i = cmd.ExecuteNonQuery();
                databaseConnection.Close();
            }

            catch (Exception ex)
            {
                Console.WriteLine("error " + ex.Message);
                
            }

            

        }



        public string ConnectionString { get => connectionString; set => connectionString = value; }
        public int NumEsp32 { get => numEsp32; set => numEsp32 = value; }
    }
}
