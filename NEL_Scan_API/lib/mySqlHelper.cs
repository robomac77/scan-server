using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
using System.Text;
using NEL_Scan_API.RPC;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;

namespace NEL_Scan_API.lib
{
	public class mySqlHelper
	{
		public static string conf = "database=block;server=106.15.200.244;user id=root;Password=jingmian@mysql;sslmode=None";

		
		public JArray GetAddress(JsonRPCrequest req)
		{
			using (MySqlConnection conn = new MySqlConnection(conf))
			{
				conn.Open();
				var addr = req.@params[0].ToString();
			
				string select = "select firstuse , lastuse , txcount from  address where addr = @addr";

				JsonPRCresponse res = new JsonPRCresponse();
				MySqlCommand cmd = new MySqlCommand(select, conn);
				cmd.Parameters.AddWithValue("@addr", addr);
				
				MySqlDataReader rdr = cmd.ExecuteReader();
				while (rdr.Read())
				{

					var adata = (rdr["firstuse"]).ToString();
					var ldata = (rdr["lastuse"]).ToString();
					var tdata = (rdr["txcount"]).ToString();

					JArray bk = new JArray {
					new JObject    {
										{"firstuse",adata}
								   },
					new JObject    {
										{"lastuse",ldata}
								   },
					new JObject    {
										{"txcount",tdata}
								   }

							   };

					res.result = bk;
				}

				return res.result;

			}
		}

		public JArray GetRankByAsset(JsonRPCrequest req)
		{
			using (MySqlConnection conn = new MySqlConnection(conf))
			{
				conn.Open();


				{
					string select = "select id, amount , admin from asset where id='" + req.@params[0] + "'";

					JsonPRCresponse res = new JsonPRCresponse();
					MySqlCommand cmd = new MySqlCommand(select, conn);

					JArray bk = new JArray();
					MySqlDataReader rdr = cmd.ExecuteReader();
					while (rdr.Read())
					{

						var adata = (rdr["id"]).ToString();
						var bl = (rdr["amount"]).ToString();
						var ad = (rdr["admin"]).ToString();

						bk.Add(new JObject { { "asset", adata }, { "balance", bl }, { "addr", ad } });

					}

					return res.result = bk;
				}
			}
		}

		public JArray GetRankByAssetCount(JsonRPCrequest req)
		{
			using (MySqlConnection conn = new MySqlConnection(conf))
			{
				conn.Open();
				//var addr = req.@params[0].ToString();

				{
					string select = "select count(*) from asset where id='" + req.@params[0].ToString() + "'";

					JsonPRCresponse res = new JsonPRCresponse();
					MySqlCommand cmd = new MySqlCommand(select, conn);


					MySqlDataReader rdr = cmd.ExecuteReader();
					while (rdr.Read())
					{

						var adata = (rdr["count(*)"]).ToString();

						JArray bk = new JArray {
					new JObject    {
										{"count",adata}
								   }

							   };

						res.result = bk;
					}



					return res.result;
				}
			}
		}

		public JArray GetAddressTx(JsonRPCrequest req)
		{
			using (MySqlConnection conn = new MySqlConnection(conf))
			{
				conn.Open();
				var addr = req.@params[0].ToString();

				string select = "select a.txid,a.addr,a.blocktime,a.blockindex,b.type,b.vout,b.vin from  address_tx as a , tx as b where @addr = addr and a.txid = b.txid limit " + req.@params[1];

				JsonPRCresponse res = new JsonPRCresponse();
				MySqlCommand cmd = new MySqlCommand(select, conn);
				cmd.Parameters.AddWithValue("@addr", addr);


				MySqlDataReader rdr = cmd.ExecuteReader();
				JArray bk = new JArray();
				while (rdr.Read())
				{

					var adata = (rdr["txid"]).ToString();
					var vdata = (rdr["addr"]).ToString();
					var bt = (rdr["blocktime"]).ToString();
					var bi = (rdr["blockindex"]).ToString();
					var type = (rdr["type"]).ToString();
					var vout = (rdr["vout"]).ToString();
					var vin = (rdr["vin"]).ToString();

					JObject t = new JObject() { { "$date", bt } };
					JObject vo = new JObject() { { "$date", bt } };
					bk.Add(new JObject { { "addr", vdata }, { "txid", adata }, { "blockindex", bi }, { "blocktime", t }, { "type", type }, { "vout", JArray.Parse(vout) }, { "vin", JArray.Parse(vin) } });

				}
				JArray c = new JArray() { };
				c.Add(new JObject { { "count", JToken.Parse("10") }, { "list", bk } });
				return res.result = c;

			}
		}


		public JArray GetAsset(JsonRPCrequest req)
		{
			using (MySqlConnection conn = new MySqlConnection(conf))
			{
				

				conn.Open();
				var available = req.@params[0].ToString();

				string select = "select id from  asset where available = @available";

				MySqlCommand cmd = new MySqlCommand(select, conn);
				cmd.Parameters.AddWithValue("@available", available);

				JsonPRCresponse res = new JsonPRCresponse();
				MySqlDataReader rdr = cmd.ExecuteReader();

				while (rdr.Read())
				{

					var adata = (rdr["id"]).ToString();

					JArray bk = new JArray {
					new JObject    {
										{"id",adata}
								   } 
					
							   };

					res.result = bk;
				}

				return res.result;

			}
		}

		public JArray GetAllAsset(JsonRPCrequest req)
		{
			using (MySqlConnection conn = new MySqlConnection(conf))
			{
				conn.Open();

				string select = "select * from  asset";
				MySqlDataAdapter adapter = new MySqlDataAdapter(select, conf);
				DataSet ds = new DataSet();
				adapter.Fill(ds);

				var data = ds.ToString();
				var alldata = Newtonsoft.Json.Linq.JArray.Parse(data);
				JsonPRCresponse res = new JsonPRCresponse();

				res.jsonrpc = req.jsonrpc;
				res.id = req.id;
				res.result = alldata;

				return alldata;

			}
		}

		public JArray GetBlock(JsonRPCrequest req)
		{
			using (MySqlConnection conn = new MySqlConnection(conf))
			{
				JsonPRCresponse res = new JsonPRCresponse();
				conn.Open();

				var hash = req.@params[0].ToString();
				string select = "select size , version , previousblockhash , merkleroot , time , nonce , nextconsensus , script  from block where hash = @hash";

				MySqlCommand cmd = new MySqlCommand(select, conn);
				cmd.Parameters.AddWithValue("@hash", hash);


				MySqlDataReader rdr = cmd.ExecuteReader();

				while (rdr.Read())
				{

				
					var sdata = (rdr["size"]).ToString();
					var adata = (rdr["version"]).ToString();
					var pdata = (rdr["previousblockhash"]).ToString();
					var mdata = (rdr["merkleroot"]).ToString();
					var tdata = (rdr["time"]).ToString();
					var ndata = (rdr["nonce"]).ToString();
					var nc = (rdr["nextconsensus"]).ToString();

					JArray bk = new JArray {
					new JObject    {
										{"size",sdata}
								   } ,
					new JObject    {
										{"version",adata}
								   },
					new JObject    {
										{"previoushash",pdata}
								   },
					new JObject    {
										{"merkleroot",mdata}
								   },
					new JObject    {
										{"time",tdata}
								   },
					new JObject    {
										{"nonce",ndata}
								   },
					new JObject    {
										{"nextconsensus",nc}
								   }
							   };

					res.result = bk;
				}

				return res.result;



			}
		}


		public JArray GetBlocks(JsonRPCrequest req)   // needs to be changed to display first 10 blocks
		{
			using (MySqlConnection conn = new MySqlConnection(conf))
			{
				JsonPRCresponse res = new JsonPRCresponse();
				conn.Open();

				var hash = req.@params[0].ToString();
				string select = "select size , version , previousblockhash , merkleroot , time , nonce , nextconsensus , script  from block where hash = @hash";

				MySqlCommand cmd = new MySqlCommand(select, conn);
				cmd.Parameters.AddWithValue("@hash", hash);
				

				MySqlDataReader rdr = cmd.ExecuteReader();

				while (rdr.Read())
				{


					var sdata = (rdr["size"]).ToString();
					var adata = (rdr["version"]).ToString();
					var pdata = (rdr["previousblockhash"]).ToString();
					var mdata = (rdr["merkleroot"]).ToString();
					var tdata = (rdr["time"]).ToString();
					var ndata = (rdr["nonce"]).ToString();
					var nc = (rdr["nextconsensus"]).ToString();

					JArray bk = new JArray {
					new JObject    {
										{"size",sdata}
								   },
					new JObject    {
										{"version",adata}
								   },
					new JObject    {
										{"previoushash",pdata}
								   },
					new JObject    {
										{"merkleroot",mdata}
								   },
					new JObject    {
										{"time",tdata}
								   },
					new JObject    {
										{"nonce",ndata}
								   },
					new JObject    {
										{"nextconsensus",nc}
								   }
							   };

					res.result = bk;
				}

				return res.result;


			}
		}

		public JArray GetNep5Asset(JsonRPCrequest req)
		{
			using (MySqlConnection conn = new MySqlConnection(conf))
			{
			

				conn.Open();
				var assetid = req.@params[0].ToString();

				string select = "select totalsupply , name , symbol , decimals from nep5asset where assetid = @assetid";

				MySqlCommand cmd = new MySqlCommand(select, conn);
				cmd.Parameters.AddWithValue("@assetid", assetid);

				
				JsonPRCresponse res = new JsonPRCresponse();

				MySqlDataReader rdr = cmd.ExecuteReader();
				while (rdr.Read())
				{

					var adata = (rdr["totalsupply"]).ToString();
					var ndata = (rdr["name"]).ToString();
					var sdata = (rdr["symbol"]).ToString();
					var ddata = (rdr["decimals"]).ToString();

					JArray bk = new JArray {
					new JObject    {
										{"totalsupply",adata}
								   },
					new JObject    {
										{"name",ndata}
								   },
					new JObject    {
										{"symbol",sdata}
								   },
					new JObject    {
										{"decimals",ddata}
								   }

							   };

					res.result = bk;
				}

				return res.result;

			}
		}

		public JArray GetAllNep5Asset(JsonRPCrequest req)
		{
			using (MySqlConnection conn = new MySqlConnection(conf))
			{
				conn.Open();
				
				string select = "select * from  nep5asset";
				MySqlDataAdapter adapter = new MySqlDataAdapter(select, conf);
				DataSet ds = new DataSet();
				adapter.Fill(ds);

				var data = ds.ToString();
				var alldata = Newtonsoft.Json.Linq.JArray.Parse(data);
				JsonPRCresponse res = new JsonPRCresponse();

				res.jsonrpc = req.jsonrpc;
				res.id = req.id;
				res.result = alldata;

				return alldata;

			}
		}

		public JArray GetNep5Transfer(JsonRPCrequest req)
		{
			using (MySqlConnection conn = new MySqlConnection(conf))
			{
			
				conn.Open();
				var txid = req.@params[0].ToString();

				string select = "select blockindex, n , asset , from , to , value from nep5transfer where txid = @txid";

				MySqlCommand cmd = new MySqlCommand(select, conn);
				cmd.Parameters.AddWithValue("@txid", txid);

	
				JsonPRCresponse res = new JsonPRCresponse();

				MySqlDataReader rdr = cmd.ExecuteReader();
				while (rdr.Read())
				{
					var bdata = (rdr["blockindex"]).ToString();
					var ndata = (rdr["n"]).ToString();
					var adata = (rdr["asset"]).ToString();
					var fdata = (rdr["from"]).ToString();
					var tdata = (rdr["to"]).ToString();
					var vdata = (rdr["value"]).ToString();

					JArray bk = new JArray {
					new JObject    {
										{"blockindex",bdata}
								   },
					new JObject    {
										{"n",ndata}
								   },
					new JObject    {
										{"asset",adata}
								   },
					new JObject    {
										{"from",fdata}
								   },
					new JObject    {
										{"to",tdata}
								   },
					new JObject    {
										{"value",vdata}
								   },


							   };

					res.result = bk;
				}

				return res.result;

			}
		}

		public JArray GetAllNep5Transfers(JsonRPCrequest req)
		{
			using (MySqlConnection conn = new MySqlConnection(conf))
			{
				conn.Open();
				string select = "select * from  nep5transfer";
				MySqlDataAdapter adapter = new MySqlDataAdapter(select, conf);
				DataSet ds = new DataSet();
				adapter.Fill(ds);
				var data = ds.ToString();
				var alldata = Newtonsoft.Json.Linq.JArray.Parse(data);
				JsonPRCresponse res = new JsonPRCresponse();
				res.jsonrpc = req.jsonrpc;
				res.id = req.id;
				res.result = alldata;

				return alldata;



			}
		}

		public JArray GetNotify(JsonRPCrequest req)
		{
			using (MySqlConnection conn = new MySqlConnection(conf))
			{
				conn.Open();
				var txid = req.@params[0].ToString();

				string select = "select gasconsumed from notify where txid = @txid";

				MySqlCommand cmd = new MySqlCommand(select, conn);
				cmd.Parameters.AddWithValue("@txid", txid);

				JsonPRCresponse res = new JsonPRCresponse();

				MySqlDataReader rdr = cmd.ExecuteReader();
				while (rdr.Read())
				{

					var adata = (rdr["gasconsumed"]).ToString();

					JArray bk = new JArray {
					new JObject    {
										{"gasconsumed",adata}
								   }

							   };

					res.result = bk;
				}

				return res.result;



			}
		}

		public JArray GetTx(JsonRPCrequest req)
		{
			using (MySqlConnection conn = new MySqlConnection(conf))
			{
				conn.Open();

				var txid = req.@params[0].ToString();
				string select = "select vin , vout from tx where txid = @txid";

				MySqlCommand cmd = new MySqlCommand(select, conn);
				cmd.Parameters.AddWithValue("@txid", txid);

				JsonPRCresponse res = new JsonPRCresponse();

				MySqlDataReader rdr = cmd.ExecuteReader();
				while (rdr.Read())
				{

					var adata = (rdr["vin"]).ToString();
					var vdata = (rdr["vout"]).ToString();

					JArray bk = new JArray {
					new JObject    {
										{"vin",adata}
								   },
					new JObject      {
										{"vout",vdata}
								   }

							   };

					res.result = bk;
				}

				return res.result;


			}

		}
		

		public JArray GetUTXO(JsonRPCrequest req)
		{

			using (MySqlConnection conn = new MySqlConnection(conf))
			{
				JsonPRCresponse res = new JsonPRCresponse();
				conn.Open();

				var txid = req.@params[0].ToString();
				string select = "select asset , value from utxo where txid = @txid";

				MySqlCommand cmd = new MySqlCommand(select, conn);
				cmd.Parameters.AddWithValue("@txid", txid);


				MySqlDataReader rdr = cmd.ExecuteReader();

				while (rdr.Read())
				{

					var adata = (rdr["asset"]).ToString();

					var vdata = (rdr["value"]).ToString();



					JArray bk = new JArray {
					new JObject    {
										{"asset",adata}
								   } ,
					new JObject    {
										{"value",vdata}
								   }
							   };

					res.result = bk;
				}

				return res.result;
			}
		}

		public JArray GetTxCount(JsonRPCrequest req)
		{
			using (MySqlConnection conn = new MySqlConnection(conf))
			{
				conn.Open();
				var id = req.@params[0].ToString();

				string select = "select txcount from  address where id = @id";

				JsonPRCresponse res = new JsonPRCresponse();
				MySqlCommand cmd = new MySqlCommand(select, conn);
				cmd.Parameters.AddWithValue("@id", id);

				MySqlDataReader rdr = cmd.ExecuteReader();
				while (rdr.Read())
				{

					var adata = (rdr["txcount"]).ToString();


					JArray bk = new JArray {
					new JObject    {
										{"firstuse",adata}
								   }


							   };

					res.result = bk;
				}

				return res.result;

			}
		}


		public JArray GetAddrCount(JsonRPCrequest req)
		{
			using (MySqlConnection conn = new MySqlConnection(conf))
			{
				conn.Open();
				var id = req.@params[0].ToString();

				string select = "select txcount from address where id = @id";

				JsonPRCresponse res = new JsonPRCresponse();
				MySqlCommand cmd = new MySqlCommand(select, conn);
				cmd.Parameters.AddWithValue("@id", id);

				MySqlDataReader rdr = cmd.ExecuteReader();
				while (rdr.Read())
				{

					var adata = (rdr["txcount"]).ToString();


					JArray bk = new JArray {
					new JObject    {
										{"txcount",adata}
								   }


							   };

					res.result = bk;
				}

				return res.result;

			}
		}

		public JArray GetBlockCount(JsonRPCrequest req)
		{
			using (MySqlConnection conn = new MySqlConnection(conf))
			{
				conn.Open();

				var id = req.@params[0].ToString();
				string select = "select height from blockheight where @id = id";

				MySqlCommand cmd = new MySqlCommand(select, conn);
				cmd.Parameters.AddWithValue("@id", id);

				JsonPRCresponse res = new JsonPRCresponse();

				MySqlDataReader rdr = cmd.ExecuteReader();
				while (rdr.Read())
				{

					var adata = (rdr["height"]).ToString();

					JArray bk = new JArray {
					 new JObject    {
										{"height",adata}
								   }

							   };

					res.result = bk;
				}


				return res.result;
			}


		}


		public JArray GetBlockHeight( JsonRPCrequest req)
		{
			using (MySqlConnection conn = new MySqlConnection(conf))
			{
				conn.Open();

				var counter = req.@params[0].ToString();
				string select = "select lastBlockindex from sys_counter where counter = @counter";

				MySqlCommand cmd = new MySqlCommand(select, conn);
				cmd.Parameters.AddWithValue("@counter", counter);

				JsonPRCresponse res = new JsonPRCresponse();

				MySqlDataReader rdr = cmd.ExecuteReader();
				while (rdr.Read())
				{

					var adata = (rdr["lastBlockindex"]).ToString();

					JArray bk = new JArray {
					new JObject    {
										{"lastblockindex",adata}
								   } 
				
							   };

					res.result = bk;
				}

				return res.result;
			}


		}

		


		public JArray GetBlockTime(JsonRPCrequest req)
		{
			using (MySqlConnection conn = new MySqlConnection(conf))
			{

				conn.Open();
				var index = req.@params[0].ToString();

				string select = "select time from  block where index = @index";

				MySqlCommand cmd = new MySqlCommand(select, conn);
				cmd.Parameters.AddWithValue("@index", index);

				MySqlDataReader rdr = cmd.ExecuteReader();
				rdr.Read();

				Object data = (rdr["time"]);
				JArray bk = new JArray(data);

				JsonPRCresponse res = new JsonPRCresponse();
				res.result = bk;
				return res.result;

			}
		}

	}
}




