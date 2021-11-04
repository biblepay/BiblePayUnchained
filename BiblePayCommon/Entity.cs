using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static BiblePayCommon.Common;
using static BiblePayCommon.Encryption;

namespace BiblePayCommon
{

    public static class EntityHelper
    {
        public static ExpandoObject ConvertToSavedExapandableObject(object obj)
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
            PropertyInfo[] properties = obj.GetType().GetProperties(flags);
            
            ExpandoObject ECO = new ExpandoObject();
            foreach (PropertyInfo property in properties)
            {
                AddProperty(ECO, property.Name, property.GetValue(obj));
            }
            return ECO;
        }

        public static void AddProperty(ExpandoObject SavedExpandableObject, string propertyName, object propertyValue)
        {
            var eDict = SavedExpandableObject as IDictionary<string, object>;
            if (eDict.ContainsKey(propertyName))
                eDict[propertyName] = propertyValue;
            else
                eDict.Add(propertyName, propertyValue);
        }
    }


    public static class EntityCommon
    {

        public enum SERVICE_TYPE
        {
            PUBLIC_CHAIN,
            PRIVATE_CHAIN,
            PUBLIC_CACHE,
            PRIVATE_CACHE
        };

        public static Dictionary<string, bool> dictTableDirty = new Dictionary<string, bool>();
        public static Dictionary<string, List<dynamic>> dictCache = new Dictionary<string, List<dynamic>>();

        public static void EvictAllCachedTables(string sTable)
        {
            if (sTable == null)
                return;

            List<string> sKeys = new List<string>();

            foreach (KeyValuePair<string, List<dynamic>> entry in dictCache)
            {
                if (entry.Key.Contains(sTable))
                {
                    sKeys.Add(entry.Key);
                }
            }
            for (int i = 0; i < sKeys.Count; i++)
            {
                dictCache.Remove(sKeys[i]);
            }
            dictCache.Clear();
        }
        public static string GetFQTableName(bool fTestNet, string sTable)
        {
            if (sTable.Contains("testnet_"))
            {
                sTable = sTable.Replace("testnet_", "");
            }
            string sChainPrefix = fTestNet ? "testnet_" : "";
            return sChainPrefix + sTable;
        }

        public static string GetEntityName(bool fTestNet, BiblePayCommon.IBBPObject o)
        {
            string sNewEntity = o.GetType().ToString();
            sNewEntity = sNewEntity.Replace("BiblePayCommon.Entity+", "");
            string sChainPrefix = fTestNet ? "testnet_" : "";
            string sTable = sChainPrefix + sNewEntity;
            return sTable;
        }


        public static List<string> GetAssemblyNameContainingType(String typeName, string sEntityName)
        {
            List<string> l = new List<string>();

            foreach (Assembly currentassembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type t = currentassembly.GetType(typeName, false, true);
                if (t != null)
                {
                    foreach (var t1 in currentassembly.GetTypes())
                    {
                        if (t1.FullName.Contains("Entity+"))
                        {
                            string class1 = t1.FullName.Replace("BiblePayCommon.Entity+", "");
                            l.Add(class1);
                            if (sEntityName == class1)
                            {
                                List<string> l1 = GetEntityPropertyNames(t1);
                                return l1;
                            }
                        }

                    }
                }
            }
            return l;
        }

        public static object GetInstance(string strFullyQualifiedName)
        {
            try
            {
                Type t = Type.GetType(strFullyQualifiedName);
                return Activator.CreateInstance(t);
            }
            catch(Exception ex)
            {
                return null;
            }
        }
        public static object GetEntityValue(object src, string propName)
        {
            try
            {
                object value = src.GetType().GetProperty(propName).GetValue(src, null);
                return value;
            }
            catch(Exception ex)
            {
                return null;
            }

        }
        public static void SetEntityValue(object o, string sColName, object oNewValue)
        {
            PropertyInfo propertyInfo = o.GetType().GetProperty(sColName);
            if (propertyInfo != null)
            {
                string sPropType = propertyInfo.PropertyType.ToString();
                if (sPropType == "System.Int32" || sPropType == "System.Int64" || sPropType == "System.Double")
                {
                    if (oNewValue.ToString() == "")
                        oNewValue = 0;
                    // If they are sending a BOOL into an int32/int64 field:
                    if (oNewValue.ToString().ToLower() == "false" || oNewValue.ToString().ToLower() == "true")
                    {
                        oNewValue = Convert.ToBoolean(oNewValue);
                    }
                }
                if (sPropType == "System.Boolean")
                {
                    if (oNewValue.ToString() == "")
                        oNewValue = false;
                }
                propertyInfo.SetValue(o, Convert.ChangeType(oNewValue, propertyInfo.PropertyType), null);
            }
            string t1 = "";

        }
        public static dynamic CastObjectToExpando(object o)
        {
            dynamic expando = new System.Dynamic.ExpandoObject();
            var dictionary = (IDictionary<string, object>)expando;
            foreach (var property in o.GetType().GetProperties())
            {
                dictionary.Add(property.Name, property.GetValue(o));
            }
            return expando;
        }

        public static void CastExpandoToBiblePayObject(ExpandoObject oExpandoObject, ref object oBiblePayObject)
        {
            try
            {
                foreach (var attribute in oExpandoObject)
                {
                    string sColName1 = attribute.Key;
                    dynamic oValue = attribute.Value;
                    if (sColName1 == "_id")
                    {
                        sColName1 = "id";
                    }
                    SetEntityValue(oBiblePayObject, sColName1, oValue);

                }
            }
            catch (Exception ex)
            {
                Log2("CastExpandoToBiblePayObject " + ex.Message);
            }
        }

        public static BiblePayCommon.IBBPObject ExpandoToStronglyCastObject(ExpandoObject oDSQL, string sTable)
        {
            BiblePayCommon.IBBPObject o = (BiblePayCommon.IBBPObject)BiblePayCommon.EntityCommon.GetInstance("BiblePayCommon.Entity+" + sTable);
            try
            {
                foreach (var attribute in oDSQL)
                {
                    string sColName1 = attribute.Key;
                    dynamic oValue = attribute.Value;
                    if (sColName1 == "_id")
                    {
                        sColName1 = "id";
                    }
                    SetEntityValue(o, sColName1, oValue);

                }
                return o;
            }
            catch (Exception ex)
            {
                return o;
            }
        }

        public static BiblePayCommon.IBBPObject TableRowToStronglyCastObject(DataTable dt, string sTable, int iRow)
        {
            BiblePayCommon.IBBPObject o = (BiblePayCommon.IBBPObject)BiblePayCommon.EntityCommon.GetInstance("BiblePayCommon.Entity+" + sTable);
            string sColName = "";
            try
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    sColName = dt.Columns[i].ColumnName;
                    object oOrigValue = dt.Rows[iRow][dt.Columns[i].ColumnName];
                    if (oOrigValue == System.DBNull.Value)
                    {
                        oOrigValue = "";
                    }
                    SetEntityValue(o, sColName, oOrigValue);
                }
                return o;
            }
            catch(Exception ex)
            {
                Log2("TRTSCO::" + ex.Message + " for " + sTable + ", " + sColName);
                return o;
            }
        }

        public static Dictionary<string, string> GetStaticFieldValues(object obj)
        {
            return obj.GetType()
                      .GetFields(BindingFlags.Public | BindingFlags.Static)
                      .Where(f => f.FieldType == typeof(string))
                      .ToDictionary(f => f.Name,
                                    f => (string)f.GetValue(null));
        }

        public static List<string> GetEntityPropertyNames(Type t)
        {
            // get all public static properties of MyClass type
            PropertyInfo[] propertyInfos;
            propertyInfos = t.GetProperties();
            List<string> lProps = new List<string>();
            Array.Sort(propertyInfos,
                    delegate (PropertyInfo propertyInfo1, PropertyInfo propertyInfo2)
                    { return propertyInfo1.Name.CompareTo(propertyInfo2.Name); });
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                lProps.Add(propertyInfo.Name);
                string sPropType = propertyInfo.PropertyType.ToString();
            }
            return lProps;
        }

        public static List<string> GetBiblePayEntities()
        {
            List<string> s = GetAssemblyNameContainingType("BiblePayCommon.Entity", "");
            return s;
        }

        public static string RESTRICTED_FIELDS = "extrakey,_id,updated,chain,deleted,guid,hash,lastblockhash,signature,signingkey,signaturetime,height,time,id,userid,serversignature,"
            + "serversigningkey,serversignaturetime,domain,parentid";
        public static string HIDDEN_FIELDS = "filename,fid,url2,transcriptjobid,svid,attachment,subject,organization";
        public static string READONLY_FIELDS = "duration,size,url,classification";

        private static bool InList(string[] vList, string sTarget)
        {
            for (int i = 0; i < vList.Length; i++)
            {
                if (vList[i].ToLower() == sTarget.ToLower())
                    return true;
            }
            return false;
        }

        public static bool IsRestrictedColumn(string sName)
        {
            string[] vRestricted = RESTRICTED_FIELDS.Split(new string[] { "," }, StringSplitOptions.None);
            return InList(vRestricted, sName);
        }
        public static string GetEntityStaticProp(string sEntityName, string sPropName)
        {
            try
            {
                BiblePayCommon.IBBPObject o = (BiblePayCommon.IBBPObject)BiblePayCommon.EntityCommon.GetInstance("BiblePayCommon.Entity+" + sEntityName);

                Dictionary<string, string> _oDict = BiblePayCommon.EntityCommon.GetStaticFieldValues(o);
                string sHF = _oDict[sPropName].ToString();
                return sHF;
            }catch(Exception ex)
            {
                return "";
            }
        }
        public static bool IsHidden(string sEntityName, string sName)
        {
            try
            {
                string sHF = GetEntityStaticProp(sEntityName, "HIDDEN_FIELDS") + "," + HIDDEN_FIELDS;
                string[] vHidden = sHF.Split(new string[] { "," }, StringSplitOptions.None);
                return InList(vHidden, sName);
            }
            catch(Exception ex)
            {
                Log2("IsHidden::" + sEntityName + "?" + sName);

                return false;
            }
        }
        public static bool IsReadOnly(string sEntityName, string sName)
        {
            string sHF = GetEntityStaticProp(sEntityName, "READ_ONLY_FIELDS") + "," + READONLY_FIELDS;

            string[] vRO = sHF.Split(new string[] { "," }, StringSplitOptions.None);
            return InList(vRO, sName);
        }
    }

    public interface IBBPObject
    {
        string GetHash();
        int deleted { get; set; }
        string id { get; set; }
        long time { get; set; }
        long updated { get; set; }
        string domain { get; set; }

        string guid { get; set; }
        string signingkey { get; set; }
        string signature { get; set; }
        long signaturetime { get; set; }
        string UserID { get; set; }
        string serversigningkey { get; set; }
        string serversignature { get; set; }
        long serversignaturetime { get; set; }
        int height { get; set; }
        int Attachment { get; set; }
        string organization { get; set; }

        string lastblockhash { get; set; }
        string nextblockhash { get; set; }
        string hash { get; set; }
        string chain { get; set; }
        string extrakey { get; set; }
    
    }

    public static class Entity
    {
        public class BaseEntity : IBBPObject
        {
            /* The following internal fields are set automatically by the biblepay.dll; do not alter these or the record will not be saved properly in the chain */
            public int deleted { get; set; }
            public string id { get; set; }
            public string chain { get; set; }
            public string extrakey { get; set; }
            public long time { get; set; }
            public string lastblockhash { get; set; }
            public string nextblockhash { get; set; }

            public int Attachment { get; set; }
            public string organization { get; set; }
            public string guid { get; set; }
            public string hash { get; set; }
            public string UserID { get; set; }
            public string signingkey { get; set; }
            public string signature { get; set; }
            public long signaturetime { get; set; }
            public string serversigningkey { get; set; }
            public string serversignature { get; set; }
            public long serversignaturetime { get; set; }
            public int height { get; set; }
            public long updated { get; set; }
            public string domain { get; set; }
            public string ParentID { get; set; }

            /* End of BiblePay internal fields */


            public virtual string GetHash()
            {
                return GetSha256HashI(id);
            }
        }
        public class comment1 : BaseEntity, IBBPObject
        {
            public string Body { get; set; }
            public override string GetHash()
            {
                return GetSha256HashI(ParentID + UserID + Body);
            }
        }

        public class video1 : BaseEntity, IBBPObject
        {
            public static string HIDDEN_FIELDS = "nextblockhash,Attachment,organization,FileName,FID,URL,URL2,Transcript,TranscriptJobID,Transcripted2,VoteSum,WatchSum,SVID,Size,Order";
            public static string READ_ONLY_FIELDS = "Duration,Size";

            public string Body { get; set; }
            public string Title { get; set; }
            public string FileName { get; set; }
            public string FID { get; set; }
            public string URL { get; set; }
            public string URL2 { get; set; }
            public string Transcript { get; set; }
            public string TranscriptJobID { get; set; }
            public int Transcripted2 { get; set; }
            public string Classification { get; set; }
            public int VoteSum { get; set; }
            public int WatchSum { get; set; }
            public string SVID { get; set; }
            public long Duration { get; set; }
            public int Size { get; set; }
            public double Order { get; set; }
            public string HashTags { get; set; }
            public string Category { get; set; }
            public string Subject { get; set; } //to be removed in favor of Category, Title and Body
            public override string GetHash()
            {
                return GetSha256HashI(ParentID + UserID + URL);
            }

            public video1()
            {
                deleted = 0;
            }

        }
        
        public class FriendRequest : BaseEntity, IBBPObject
        {
            public string RequesterID { get; set; }
            public override string GetHash()
            {
                return GetSha256HashI(RequesterID + UserID);
            }
        }

        public class Friend : BaseEntity, IBBPObject
        {
            public string RequesterID { get; set; }

            public override string GetHash()
            {
                return GetSha256HashI(RequesterID + UserID);
            }
        }

        public class Timeline : BaseEntity, IBBPObject
        {
            public string Subject { get; set; }
            public string Body { get; set; }
            public string Privacy { get; set; }
            public string URL { get; set; }
            public string URLTitle { get; set; }
            public string URLDescription { get; set; }
            public string URLPreviewImage { get; set; }
            
            public override string GetHash()
            {
                return GetSha256HashI(Subject + Body + UserID);
            }
        }

        public class Organization: BaseEntity, IBBPObject
        {
            public string Name { get; set; }
            public string Domain { get; set; }

            public override string GetHash()
            {
                return GetSha256HashI(Name);
            }
        }

        public class Role : BaseEntity, IBBPObject
        {
            public string OrganizationID { get; set; }
            public string Name { get; set; }
            public override string GetHash()
            {
                return GetSha256HashI(OrganizationID + Name);
            }
        }

        public class UserRole : BaseEntity, IBBPObject
        {
            public string UserGuid { get; set; }
            public string RoleID { get; set; }
            public override string GetHash()
            {
                return GetSha256HashI(UserID + RoleID);
            }
        }

        public class Permission : BaseEntity, IBBPObject
        {
            public string RoleID { get; set; }
            public string Name { get; set; }
            public string EntityName { get; set; }
            public int ReadAccess { get; set; }
            public int AddAccess { get; set; }
            public int UpdateAccess { get; set; }
            public int DeleteAccess { get; set; }
            public override string GetHash()
            {
                return GetSha256HashI(RoleID + Name);
            }
        }

        public class user1 : BaseEntity, IBBPObject
        {
            public string UserName { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string CompanyName { get; set; }
            public string BiblePayAddress { get; set; }
            public string RSAKey { get; set; }
            public string Shared2FA { get; set; }
            public string PasswordHash { get; set; }
            public string EmailAddress { get; set; }
            public int Verified { get; set; }
            public int FA2Verified { get; set; }
            public int EmailVerified { get; set; }
            public string AvatarURL { get; set; }
            public int Administrator { get; set; }
            public int Banned { get; set; }
            public int Tickets { get; set; }
            public string HashTags { get; set; }
            public string Testimony { get; set; }
            public string ThemeName { get; set; }
            public string Slogan { get; set; }
            public string UserGuid { get; set; }
            public string PublicText { get; set; }
            public string ProfessionalText { get; set; }
            public string PrivateText { get; set; }
            public string ReligiousText { get; set; }
            public string Gender { get; set; }

            public string TelegramLinkName { get; set; }
            public string TelegramLinkURL { get; set; }
            public string TelegramLinkDescription { get; set; }

            public int BirthDate { get; set; }
            public override string GetHash()
            {
                return UserGuid;
            }
        }


        public class TicketHistory: BaseEntity, IBBPObject
        {
            public static string CollectionName = "Ticket History";
            public static string ObjectName = "TicketHistory";

            public string Body { get; set; }
            public string AssignedTo { get; set; }
            public string Disposition { get; set; }
            public double Hours { get; set; }

            public override string GetHash()
            {
                return GetSha256HashI(id);
            }

        }

        public class Ticket : BaseEntity, IBBPObject
        {
            public static string CollectionName = "Ticket";
            public static string ObjectName = "Ticket";

            public string Title { get; set; }
            public int TicketNumber { get; set; }
            public string Disposition { get; set; }
            public string AssignedTo { get; set; }
            public string Body { get; set; }

            public override string GetHash()
            {
                return GetSha256HashI(ParentID + UserID + TicketNumber);
            }

        }

        public class object1 : BaseEntity, IBBPObject
        {
            public string FileName { get; set; }
            public string Subject { get; set; }
            public string Title { get; set; }
            public string Body { get; set; }
            public string DirectoryName { get; set; }
            public long Size { get; set; }
            public string Category { get; set; }
            public string URL { get; set; }
            public int ExpirationTime { get; set; }
            public override string GetHash()
            {
                return GetSha256HashI(UserID + DirectoryName + FileName + Title + Body + Subject);
            }
        }

        public class news1 : BaseEntity, IBBPObject
        {
            public string Title { get; set; }
            public string URL { get; set; }
            public override string GetHash()
            {
                return GetSha256HashI(URL);
            }
        }

        public class VideoRequest : BaseEntity, IBBPObject
        {
            public static string HIDDEN_FIELDS = "Processed,URL2,ProcessTime";

            public string URL { get; set; }
            // URL is the Requested URL
            // URL2 is the Output PROCESSED URL ready for viewing
            public string URL2 { get; set; }
            public string Processed { get; set; }
            public int ProcessTime { get; set; }

            public override string GetHash()
            {
                return GetSha256HashI(URL);
            }
        }


        public class versememorizer1 : BaseEntity, IBBPObject
        {
            public string BookFrom { get; set; }
            public int ChapterFrom { get; set; }
            public int VerseFrom { get; set; }
            public string BookTo { get; set; }
            public int ChapterTo { get; set; }
            public int VerseTo { get; set; }

            public override string GetHash()
            {
                return GetSha256HashI(BookFrom + ChapterFrom.ToString() + VerseFrom.ToString());
            }
        }
        public class vote1 : BaseEntity, IBBPObject
        {
            public string VoteType { get; set; }
            public int VoteValue { get; set; }
            public override string GetHash()
            {
                return GetSha256HashI(UserID + ParentID);
            }

        }

        public class pray1 : BaseEntity, IBBPObject
        {
            public static string CollectionName = "Prayer Requests";
            public static string ObjectName = "Prayer";
            public string Subject { get; set; }
            public string Body { get; set; }
            public override string GetHash()
            {
                return GetSha256HashI(UserID + Subject + Body);
            }
        }

        public class townhall1 : BaseEntity, IBBPObject
        {
            public static string CollectionName = "Town Hall Discussion";
            public static string ObjectName = "Town Hall Discussion Item";
            public string Subject { get; set; }
            public string Body { get; set; }
            public override string GetHash()
            {
                return GetSha256HashI(UserID + Subject + Body);
            }
        }

        public class diary1 : BaseEntity, IBBPObject
        {
            public static string CollectionName = "Healing Diary Testimonies";
            public static string ObjectName = "Healing Diary Entry";

            public string Subject { get; set; }
            public string Body { get; set; }
            public override string GetHash()
            {
                return GetSha256HashI(UserID + Subject + Body);
            }
        }


        public class objectcount1 : BaseEntity, IBBPObject
        {
            public string CountType { get; set; }
            public int CountValue { get; set; }
            public override string GetHash()
            {
                return GetSha256HashI(UserID + ParentID);
            }
        }

        public class invoice1 : BaseEntity, IBBPObject
        {
            public string BillFromAddress { get; set; }
            public string BillToAddress { get; set; }
            public double Amount { get; set; }
            public string Data { get; set; }
            public string InvoiceType { get; set; }
            public string ProductID { get; set; }
            public string ServiceName { get; set; }
            public string InvoiceDate { get; set; }
            public override string GetHash()
            {
                return GetSha256HashI(BillFromAddress + BillToAddress + InvoiceDate);
            }
        }

        public class accountingobject : BaseEntity, IBBPObject
        {
            public string FileName { get; set; }
            public string Data { get; set; }
            public string BillFromAddress { get; set; }
            public string BillToAddress { get; set; }
            public double Balance { get; set; }
        }

        public class follow1 : BaseEntity, IBBPObject
        {
            public string FollowedID { get; set; }
            public string Status { get; set; }
            public override string GetHash()
            {
                return GetSha256HashI(FollowedID + UserID);
            }
            public follow1()
            {
                deleted = 0;
            }

        }

        public class utxostake1 : BaseEntity, IBBPObject
        {
            public string Address { get; set; }
            public string Ticker { get; set; }
            public int Tithe { get; set; }
            public double Amount { get; set; }
            public double USDAmount { get; set; }
            public double Coverage { get; set; }
            public string OwnerAddress { get; set; }

            public override string GetHash()
            {
                // This means that no duplicate address records can exist per user.  
                return GetSha256HashI(UserID + Address);
            }

            public utxostake1()
            {
                deleted = 0;
                Tithe = 0;
            }
        }
        public class price1 : BaseEntity, IBBPObject
        {
            public string Ticker { get; set; }
            public double Amount { get; set; }
            public double AmountUSD { get; set; }
            public price1()
            {
                deleted = 0;
            }
            public override string GetHash()
            {
                return GetSha256HashI(Ticker);
            }
        }

        public class utxointegration2 : BaseEntity, IBBPObject
        {
            public string DateTime { get; set; }
            public string Data { get; set; }
            public utxointegration2()
            {
                Data = String.Empty;
            }
            public override string GetHash()
            {
                return GetSha256HashI(DateTime + Data);
            }
        }

        public class NFTBid: BaseEntity, IBBPObject
        {
            public string NFTId { get; set; }
            public double Amount { get; set; }
            public override string GetHash()
            {
                return GetSha256HashI(UserID + NFTId + Amount);
            }

        }


        public class performance1 : BaseEntity, IBBPObject
        {
            public override string GetHash()
            {
                return GetSha256HashI(id);
            }

            public string field1 { get; set; }
            public string field2 { get; set; }
            public string field3 { get; set; }
        }


        public class NFT : BaseEntity, IBBPObject
        {
            public string Name { get;set; }
            public string Action { get; set; }
            public string Description { get; set; }
            public string LowQualityURL { get; set; }
            public string HighQualityURL { get; set; }
            public string Type { get; set; }
            public string XML { get; set; }
            public double MinimumBidAmount { get; set; }
            public double ReserveAmount { get; set; }
            public double BuyItNowAmount { get; set; }
            public string OwnerUserID { get; set; }
            public int nIteration { get; set; }
            public bool Marketable { get; set; }
            public bool fDeleted { get; set; }
            public bool found { get; set; }
            public string TXID { get; set; }
            public override string GetHash()
            {
                return GetSha256HashI(LowQualityURL);
            }

            public NFT()
            {
                Marketable = false;
                Action = "";
                UserID = "";
            }

            public double LowestAcceptableAmount()
            {
                double nAcceptable = 100000000;
                if (ReserveAmount > 0 && BuyItNowAmount > 0)
                {
                    // This is an Auction AND a buy-it-now NFT, so accept the lower of the two
                    nAcceptable = Math.Min(ReserveAmount, BuyItNowAmount);
                }
                else if (ReserveAmount > 0 && BuyItNowAmount == 0)
                {
                    // This is an auction (but not a buy it now)
                    nAcceptable = ReserveAmount;
                }
                else if (BuyItNowAmount > 0 && ReserveAmount == 0)
                {
                    nAcceptable = BuyItNowAmount;
                }
                return nAcceptable;
            }

        }
    
    }
}
