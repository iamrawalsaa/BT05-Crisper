using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using static System.Net.Mime.MediaTypeNames;


namespace shared
{
    /// <summary>
    /// A bunch of helper extension methods that can be used on other projects
    /// </summary>
    public static class ExtensionMethods
    {
        public static Random _random = new Random();


        public static string Right(this string input, int count)
        {
            return input.Substring(Math.Max(input.Length - count, 0), Math.Min(count, input.Length));
        }

        /// <summary>
        /// Returns the left part of this string instance.
        /// </summary>
        /// <param name="count">Number of characters to return.</param>
        public static string Left(this string input, int count)
        {
            return input.Substring(0, Math.Min(input.Length, count));
        }

        public static T GetRandomElement<T>(this IEnumerable<T> list)
        {
            // If there are no elements in the collection, return the default value of T
            if (list.Count() == 0)
                return default(T);

            return list.ElementAt(_random.Next(list.Count()));
        }

        const string WORDS_SOURCE = "ability,able,about,above,accept,according,account,across,act,action,activity,actually,add,address,administration,admit,adult,affect,after,again,against,age,agency,agent,ago,agree,agreement,ahead,air,all,allow,almost,alone,along,already,also,although,always,American,among,amount,analysis,and,animal,another,answer,any,anyone,anything,appear,apply,approach,area,argue,arm,around,arrive,art,article,artist,as,ask,assume,at,attack,attention,attorney,audience,author,authority,available,avoid,away,baby,back,bad,bag,ball,bank,bar,base,be,beat,beautiful,because,become,bed,before,begin,behavior,behind,believe,benefit,best,better,between,beyond,big,bill,billion,bit,black,blood,blue,board,body,book,born,both,box,boy,break,bring,brother,budget,build,building,business,but,buy,by,call,camera,campaign,can,cancer,candidate,capital,car,card,care,career,carry,case,catch,cause,cell,center,central,century,certain,certainly,chair,challenge,chance,change,character,charge,check,child,choice,choose,church,citizen,city,civil,claim,class,clear,clearly,close,coach,cold,collection,college,color,come,commercial,common,community,company,compare,computer,concern,condition,conference,Congress,consider,consumer,contain,continue,control,cost,could,country,couple,course,court,cover,create,crime,cultural,culture,cup,current,customer,cut,dark,data,daughter,day,dead,deal,death,debate,decade,decide,decision,deep,defense,degree,Democrat,democratic,describe,design,despite,detail,determine,develop,development,die,difference,different,difficult,dinner,direction,director,discover,discuss,discussion,disease,do,doctor,dog,door,down,draw,dream,drive,drop,drug,during,each,early,east,easy,eat,economic,economy,edge,education,effect,effort,eight,either,election,else,employee,end,energy,enjoy,enough,enter,entire,environment,environmental,especially,establish,even,evening,event,ever,every,everybody,everyone,everything,evidence,exactly,example,executive,exist,expect,experience,expert,explain,eye,face,fact,factor,fail,fall,family,far,fast,father,fear,federal,feel,feeling,few,field,fight,figure,fill,film,final,finally,financial,find,fine,finger,finish,fire,firm,first,fish,five,floor,fly,focus,follow,food,foot,for,force,foreign,forget,form,former,forward,four,free,friend,from,front,full,fund,future,game,garden,gas,general,generation,get,girl,give,glass,go,goal,good,government,great,green,ground,group,grow,growth,guess,gun,guy,hair,half,hand,hang,happen,happy,hard,have,he,head,health,hear,heart,heat,heavy,help,her,here,herself,high,him,himself,his,history,hit,hold,home,hope,hospital,hot,hotel,hour,house,how,however,huge,human,hundred,husband,I,idea,identify,if,image,imagine,impact,important,improve,in,include,including,increase,indeed,indicate,individual,industry,information,inside,instead,institution,interest,interesting,international,interview,into,investment,involve,issue,it,item,its,itself,job,join,just,keep,key,kid,kill,kind,kitchen,know,knowledge,land,language,large,last,late,later,laugh,law,lawyer,lay,lead,leader,learn,least,leave,left,leg,legal,less,let,letter,level,lie,life,light,like,likely,line,list,listen,little,live,local,long,look,lose,loss,lot,love,low,machine,magazine,main,maintain,major,majority,make,man,manage,management,manager,many,market,marriage,material,matter,may,maybe,me,mean,measure,media,medical,meet,meeting,member,memory,mention,message,method,middle,might,military,million,mind,minute,miss,mission,model,modern,moment,money,month,more,morning,most,mother,mouth,move,movement,movie,Mr,Mrs,much,music,must,my,myself,name,nation,national,natural,nature,near,nearly,necessary,need,network,never,new,news,newspaper,next,nice,night,no,none,nor,north,not,note,nothing,notice,now,n't,number,occur,of,off,offer,office,officer,official,often,oh,oil,ok,old,on,once,one,only,onto,open,operation,opportunity,option,or,order,organization,other,others,our,out,outside,over,own,owner,page,pain,painting,paper,parent,part,participant,particular,particularly,partner,party,pass,past,patient,pattern,pay,peace,people,per,perform,performance,perhaps,period,person,personal,phone,physical,pick,picture,piece,place,plan,plant,play,player,PM,point,police,policy,political,politics,poor,popular,population,position,positive,possible,power,practice,prepare,present,president,pressure,pretty,prevent,price,private,probably,problem,process,produce,product,production,professional,professor,program,project,property,protect,prove,provide,public,pull,purpose,push,put,quality,question,quickly,quite,race,radio,raise,range,rate,rather,reach,read,ready,real,reality,realize,really,reason,receive,recent,recently,recognize,record,red,reduce,reflect,region,relate,relationship,religious,remain,remember,remove,report,represent,Republican,require,research,resource,respond,response,responsibility,rest,result,return,reveal,rich,right,rise,risk,road,rock,role,room,rule,run,safe,same,save,say,scene,school,science,scientist,score,sea,season,seat,second,section,security,see,seek,seem,sell,send,senior,sense,series,serious,serve,service,set,seven,several,sex,sexual,shake,share,she,shoot,short,shot,should,shoulder,show,side,sign,significant,similar,simple,simply,since,sing,single,sister,sit,site,situation,six,size,skill,skin,small,smile,so,social,society,soldier,some,somebody,someone,something,sometimes,son,song,soon,sort,sound,source,south,southern,space,speak,special,specific,speech,spend,sport,spring,staff,stage,stand,standard,star,start,state,statement,station,stay,step,still,stock,stop,store,story,strategy,street,strong,structure,student,study,stuff,style,subject,success,successful,such,suddenly,suffer,suggest,summer,support,sure,surface,system,table,take,talk,task,tax,teach,teacher,team,technology,television,tell,ten,tend,term,test,than,thank,that,the,their,them,themselves,then,theory,there,these,they,thing,think,third,this,those,though,thought,thousand,threat,three,through,throughout,throw,thus,time,to,today,together,tonight,too,top,total,tough,toward,town,trade,traditional,training,travel,treat,treatment,tree,trial,trip,trouble,true,truth,try,turn,TV,two,type,under,understand,unit,until,up,upon,us,use,usually,value,various,very,victim,view,violence,visit,voice,vote,wait,walk,wall,want,war,watch,water,way,we,weapon,wear,week,weight,well,west,western,what,whatever,when,where,whether,which,while,white,who,whole,whom,whose,why,wide,wife,will,win,wind,window,wish,with,within,without,woman,wonder,word,work,worker,world,worry,would,write,writer,wrong,yard,yeah,year,yes,yet,you,young,your,yourself";
        //const string WORDS_SOURCE = "ability,able,about,above,accept,according,account,across,act,action,activity,actually,add,address,administration,admit,adult,affect,after,again,against,age,agency,agent,ago,agree,agreement,ahead,air,all,allow,almost,alone,along,already,also,although,always,American,among,amount,analysis,and,animal,another,answer,any,anyone,anything,appear,apply,approach,area,argue,arm,around,arrive,art,article,artist,as,ask,assume,at,attack,attention,attorney,audience,author,authority,available";

        static List<string> _wordList = null;

        const string COLOURS_SOURCE = "RED,BLUE,GREEN,YELLOW,BROWN,BLACK,WHITE,CYAN,PINK,GRAY,MAGENTA,ORANGE,PURPLE,SILVER,GOLD,TEAL,NAVY,L_RED,L_BLUE,L_GREEN,L_YELLOW,L_BROWN,L_CYAN,L_PINK,L_GRAY,L_GOLD,D_RED,D_BLUE,D_GREEN,D_YELLOW,D_BROWN,D_CYAN,D_PINK,D_GRAY,D_GOLD";
        static List<string> _colourList = null;


        public static string GenerateRandomSentence( )
        {
            var sentenceLength = _random.Next( 5,40 );

            string sentence = "";

            if (_wordList == null)
            {
                var split = WORDS_SOURCE.Split(',');
                _wordList = new List<string>(split);
            }

            if (_colourList == null)
            {
                var split = COLOURS_SOURCE.Split(',');
                _colourList = new List<string>(split);
            }

            for(int i =0; i < sentenceLength; ++i)
            {
                var randomIndex = _random.Next(_wordList.Count);
                sentence += _wordList[randomIndex] + " ";
            }

            return sentence.Trim();
        }

        public static string GenerateRandomSentenceWithColourTagsAndNewLines()
        {
            var sentenceLength = _random.Next(5, 40);

            string sentence = "";

            if (_wordList == null)
            {
                var split = WORDS_SOURCE.Split(',');
                _wordList = new List<string>(split);
            }

            if (_colourList == null)
            {
                var split = COLOURS_SOURCE.Split(',');
                _colourList = new List<string>(split);
            }

            for (int i = 0; i < sentenceLength; ++i)
            {
                {
                    var randomIndex = _random.Next(_wordList.Count);
                    sentence += _wordList[randomIndex] + " ";
                }

                if (_random.Next(1,10) > 5)
                {
                    sentence += "NL\n";
                }

                if (_random.Next(1, 10) > 5)
                {
                    var randomIndex = _random.Next(_colourList.Count);
                    sentence += "{{"+_colourList[randomIndex] + "}}";
                }
            }

            return sentence.Trim();
        }

        /// <summary>
        /// The bracket {{ }} commands are for the color coding.  I need to remove these on the client.
        /// </summary>
        /// <returns></returns>
        public static string RemoveTheBracketCommands( string inputText )
        {
            string _styleInstructionsOpening = "{{";
            string _styleInstructionsClosing = "}}";

            System.Text.RegularExpressions.Regex _styleInstructionsRegex = new System.Text.RegularExpressions.Regex(_styleInstructionsOpening + "[^{}]*" + _styleInstructionsClosing);
            
            var openingDenote = _styleInstructionsOpening;
            var closingDenote = _styleInstructionsClosing;
            var denotesLength = (openingDenote.Length + closingDenote.Length);

            string returnString = "";

            if (inputText.Contains(openingDenote))
            {
                int iLastPosition = 0;

                System.Text.RegularExpressions.MatchCollection oMatches = _styleInstructionsRegex.Matches(inputText);
                foreach (System.Text.RegularExpressions.Match oMatch in oMatches)
                {
                    var sectionText = inputText.Substring(iLastPosition, oMatch.Index - iLastPosition);

                    if (!string.IsNullOrEmpty(sectionText))
                    {
                        returnString += sectionText;
                    }

                    string key = oMatch.Value.Substring(openingDenote.Length, oMatch.Value.Length - denotesLength);
                    iLastPosition = oMatch.Index + oMatch.Value.Length;
                }

                var remainingText = inputText.Substring(iLastPosition);
                if (!string.IsNullOrWhiteSpace(remainingText))
                {
                    returnString += remainingText;
                }
            }
            else
            {
                // if there's no commands - then do still copy the text across
                returnString = inputText;
            }

            return returnString;
        }

        const string BuildVersionMetadataPrefix = "+build";
        const string dateFormat = "yyyy-MM-ddTHH:mm:ss:fffZ";

        public static IPAddress GetOurLocalIPAddress()
        {
            List<string> ips = new List<string>();
            System.Net.IPHostEntry entry = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
            foreach (System.Net.IPAddress ip in entry.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    ips.Add(ip.ToString());
                    return ip;
                    //DebugOutput.Instance.WriteInfo("Our local IP: "+ ip.ToString());
                }
            }

            return IPAddress.Any;
        }

        public static bool IsValidIPAddress(string IPAddress)
        {
            if (string.IsNullOrEmpty(IPAddress)) return false;

            var x = IPAddress.Split('.');

            if (x == null) return false;

            if (x.Length != 4)
                return false;

            foreach (var i in x)
            {
                int q;
                if (!Int32.TryParse(i, out q) || q < 0 || q > 255)
                {
                    return false;
                }
            }

            return true;
        }

        public static DateTime GetLinkerTime(Assembly assembly)
        {
            var attribute = assembly
              .GetCustomAttribute<AssemblyInformationalVersionAttribute>();

            if (attribute?.InformationalVersion != null)
            {
                var value = attribute.InformationalVersion;
                var index = value.IndexOf(BuildVersionMetadataPrefix);
                if (index > 0)
                {
                    value = value[(index + BuildVersionMetadataPrefix.Length)..];

                    return DateTime.ParseExact(
                        value,
                      dateFormat,
                      CultureInfo.InvariantCulture);
                }
            }
            return default;
        }


        /// <summary>
        /// This is a remap function to map between an old range and a new range.
        /// The aValue is the old range
        /// </summary>
        /// <param name="aValue"></param>
        /// <param name="aIn1"></param>
        /// <param name="aIn2"></param>
        /// <param name="aOut1"></param>
        /// <param name="aOut2"></param>
        /// <returns></returns>
        public static double Remap(this double aValue, double aIn1, double aIn2, double aOut1, double aOut2)
        {
            double t = (aValue - aIn1) / (aIn2 - aIn1);
            return aOut1 + (aOut2 - aOut1) * t;
        }

        public static float Remap(this float aValue, float aIn1, float aIn2, float aOut1, float aOut2)
        {
            float t = (aValue - aIn1) / (aIn2 - aIn1);
            return aOut1 + (aOut2 - aOut1) * t;
        }

        public static int Remap(this int aValue, int aIn1, int aIn2, int aOut1, int aOut2)
        {
            float t = (aValue - aIn1) / (float)(aIn2 - aIn1);
            float result = (aOut1 + (aOut2 - aOut1) * t);
            return (int)result;
        }

        public static T ToEnum<T>(this object obj)
        {
            var objType = obj.GetType();
            if (typeof(T).IsEnum)
            {
                if (objType == typeof(string))
                    return (T)Enum.Parse(typeof(T), obj.ToString());
                return (T)Enum.ToObject(typeof(T), obj);
            }
            if (objType == typeof(string))
                return (T)Enum.Parse(Nullable.GetUnderlyingType(typeof(T)), obj.ToString());
            return (T)Enum.ToObject(Nullable.GetUnderlyingType(typeof(T)), obj);
        }



        /// <summary>
        /// This formats the number in terms of millions or billions. Makes it more readable for players.
        /// </summary>
        /// <param name="carbonTonnes"></param>
        /// <returns></returns>
        public static string ToMillionsOrBillions(this double carbonTonnesBillions)
        {
            string output = "";

            if (carbonTonnesBillions >= 1.0)
            {
                output = carbonTonnesBillions.ToString("0.0") + " billion";
            }
            else
            {
                double carbonTonnesMillions = carbonTonnesBillions * 1000;
                output = carbonTonnesMillions.ToString("0") + " million";
            }

            return output;
        }

        public static string AddSpacingTo(this string input, int maxSize)
        {
            if (input.Length >= maxSize)
            {
                return input.Substring(0, maxSize);
            }
            else
            {
                string filler = "";
                
                for(int i=0; i<maxSize-input.Length;++i)
                {
                    filler += " ";
                }
                return input + filler;
            }
        }

        public static string ToMillionsOrBillionsShort(this double carbonTonnesBillions)
        {
            string output = "";

            if (carbonTonnesBillions >= 1.0)
            {
                output = carbonTonnesBillions.ToString("0.0") + "B";
            }
            else
            {
                double carbonTonnesMillions = carbonTonnesBillions * 1000;
                output = carbonTonnesMillions.ToString("0") + "M";
            }

            return output;
        }

        public static string ToMillionsOrBillionsShortWithSpace(this double carbonTonnesBillions)
        {
            string output = "";

            if (carbonTonnesBillions >= 1.0)
            {
                output = carbonTonnesBillions.ToString("0.0") + " B";
            }
            else
            {
                double carbonTonnesMillions = carbonTonnesBillions * 1000;
                output = carbonTonnesMillions.ToString("0") + " M";
            }

            return output;
        }

        /// <summary>
        /// This formats the number in terms of millions or billions. Makes it more readable for players.
        /// </summary>
        /// <param name="carbonTonnes"></param>
        /// <returns></returns>
        //string FormatCarbon(double carbonTonnesBillions)
        //{
        //    string output = "";

        //    if (carbonTonnesBillions >= 1.0)
        //    {
        //        output = carbonTonnesBillions.ToString("0.0") + " billion";
        //    }
        //    else
        //    {
        //        double carbonTonnesMillions = carbonTonnesBillions * 1000;
        //        output = carbonTonnesMillions.ToString("0") + " million";
        //    }

        //    return output;
        //}


            public static T CopyObject<T>(this object objSource)
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    BinaryFormatter formatter = new BinaryFormatter();
#pragma warning disable SYSLIB0011
                formatter.Serialize(stream, objSource);
                    stream.Position = 0;
                    return (T)formatter.Deserialize(stream);
#pragma warning restore SYSLIB0011
            }
        }

        public static int Mod(int x, int m)
        {
            return (x % m + m) % m;
        }

    }
}
