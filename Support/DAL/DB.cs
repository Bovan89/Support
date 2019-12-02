using Support.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace Support.DAL
{
    public class DB : DbContext
    {
        public static System.Threading.Mutex DbMutex = new System.Threading.Mutex();

        public static DB Create()
        {
            return new DB();
        }

        public DB() : base("DefaultConnection")
        {
            Database.SetInitializer<DB>(new DBInitializer());
        }

        public DbSet<ClientRequest> ClientRequest { get; set; }
        public DbSet<Employee> Employee { get; set; }
        public DbSet<SupportParameter> SupportParameter { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }

    public class DBInitializer : CreateDatabaseIfNotExists<DB>
        //DropCreateDatabaseAlways<DB>
    {
        protected override void Seed(DB context)
        {
            var employee = new List<Employee>
            {
                new Employee { Name = "Вася", Title = Employee.TitleEnum.Operator },
                new Employee { Name = "Петя", Title = Employee.TitleEnum.Operator },
                new Employee { Name = "Илья", Title = Employee.TitleEnum.Operator },
                new Employee { Name = "Катя", Title = Employee.TitleEnum.Operator },
                new Employee { Name = "Галя", Title = Employee.TitleEnum.Operator },
                new Employee { Name = "Вера", Title = Employee.TitleEnum.Operator },
                new Employee { Name = "Александр", Title = Employee.TitleEnum.Manager },
                new Employee { Name = "Владимир", Title = Employee.TitleEnum.Manager },
                new Employee { Name = "Артур", Title = Employee.TitleEnum.Manager },
                new Employee { Name = "Жан", Title = Employee.TitleEnum.Manager },                
                new Employee { Name = "Павел Петрович", Title = Employee.TitleEnum.Director },
                new Employee { Name = "Денис Юрьевич", Title = Employee.TitleEnum.Director }
            };
            employee.ForEach(e => context.Employee.Add(e));

            var clientRequest = new List<ClientRequest>
            {
                new ClientRequest{Text = "text1",CreatedOn = DateTime.Now,State = Models.ClientRequest.ClientRequestState.New},
                new ClientRequest{Text = "text2",CreatedOn = DateTime.Now,State = Models.ClientRequest.ClientRequestState.New},
                new ClientRequest{Text = "text3",CreatedOn = DateTime.Now,State = Models.ClientRequest.ClientRequestState.New},
                new ClientRequest{Text = "text4",CreatedOn = DateTime.Now,State = Models.ClientRequest.ClientRequestState.New},
                new ClientRequest{Text = "text5",CreatedOn = DateTime.Now,State = Models.ClientRequest.ClientRequestState.New},
                new ClientRequest{Text = "text6",CreatedOn = DateTime.Now,State = Models.ClientRequest.ClientRequestState.New},
                new ClientRequest{Text = "text7",CreatedOn = DateTime.Now,State = Models.ClientRequest.ClientRequestState.New},
                new ClientRequest{Text = "text8",CreatedOn = DateTime.Now,State = Models.ClientRequest.ClientRequestState.New},
                new ClientRequest{Text = "text9",CreatedOn = DateTime.Now,State = Models.ClientRequest.ClientRequestState.New},
                new ClientRequest{Text = "tex10",CreatedOn = DateTime.Now,State = Models.ClientRequest.ClientRequestState.New},
                new ClientRequest{Text = "tex11",CreatedOn = DateTime.Now,State = Models.ClientRequest.ClientRequestState.New},
                new ClientRequest{Text = "tex12",CreatedOn = DateTime.Now,State = Models.ClientRequest.ClientRequestState.New},
                new ClientRequest{Text = "tex13",CreatedOn = DateTime.Now,State = Models.ClientRequest.ClientRequestState.New},
                new ClientRequest{Text = "tex14",CreatedOn = DateTime.Now,State = Models.ClientRequest.ClientRequestState.New},
                new ClientRequest{Text = "tex15",CreatedOn = DateTime.Now,State = Models.ClientRequest.ClientRequestState.New},
                new ClientRequest{Text = "tex16",CreatedOn = DateTime.Now,State = Models.ClientRequest.ClientRequestState.New},
                new ClientRequest{Text = "tex17",CreatedOn = DateTime.Now,State = Models.ClientRequest.ClientRequestState.New},
                new ClientRequest{Text = "tex18",CreatedOn = DateTime.Now,State = Models.ClientRequest.ClientRequestState.New},
                new ClientRequest{Text = "tex19",CreatedOn = DateTime.Now,State = Models.ClientRequest.ClientRequestState.New},
                new ClientRequest{Text = "tex20",CreatedOn = DateTime.Now,State = Models.ClientRequest.ClientRequestState.New},
                new ClientRequest{Text = "tex21",CreatedOn = DateTime.Now,State = Models.ClientRequest.ClientRequestState.New},
                new ClientRequest{Text = "tex22",CreatedOn = DateTime.Now,State = Models.ClientRequest.ClientRequestState.New},
                new ClientRequest{Text = "tex23",CreatedOn = DateTime.Now,State = Models.ClientRequest.ClientRequestState.New},
                new ClientRequest{Text = "tex24",CreatedOn = DateTime.Now,State = Models.ClientRequest.ClientRequestState.New},
                new ClientRequest{Text = "tex25",CreatedOn = DateTime.Now,State = Models.ClientRequest.ClientRequestState.New},
                new ClientRequest{Text = "tex26",CreatedOn = DateTime.Now,State = Models.ClientRequest.ClientRequestState.New},
                new ClientRequest{Text = "tex27",CreatedOn = DateTime.Now,State = Models.ClientRequest.ClientRequestState.New},
                new ClientRequest{Text = "tex28",CreatedOn = DateTime.Now,State = Models.ClientRequest.ClientRequestState.New},
                new ClientRequest{Text = "tex29",CreatedOn = DateTime.Now,State = Models.ClientRequest.ClientRequestState.New},
                new ClientRequest{Text = "tex30",CreatedOn = DateTime.Now,State = Models.ClientRequest.ClientRequestState.New}
            };
            clientRequest.ForEach(r => context.ClientRequest.Add(r));

            var parameter = new List<SupportParameter>
            {
                new SupportParameter { ParameterName="Tm", ParameterValue=50},
                new SupportParameter { ParameterName="Td", ParameterValue=70},
                new SupportParameter { ParameterName="E0", ParameterValue=25},
                new SupportParameter { ParameterName="E1", ParameterValue=150}
            };
            parameter.ForEach(p => context.SupportParameter.Add(p));

            context.SaveChanges();

            base.Seed(context);
        }
    }
}