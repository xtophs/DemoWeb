using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web2.Models
{
    public class CreditRiskApplication
    {
        public CreditRiskApplication()
        {
            FirstName = "";
            LastName = "";
            CheckingAccountStatus = "";
            DurationInMonths = "";
            CreditHistory = "";
            Purpose = "";
            CreditAmount = "";
            SavingsAccount = "";
            PresentEmploymentSince = "";
            InstallmentPecentageOfDI = "";
            PersonalStatusAndSex = "";
            OtherDebtors = "";
            PresentResidenceSince = "";
            Property = "";
            AgeInYears = "";
            OtherInstallments = "";
            Housing = "";
            NumberOfExistingCredits = "";
            Job = "";
            NumberOfPeopleLiable = "";
            Telephone = "";
            ForeignWorker = "";
            CreditRisk = "";
        }

        public CreditRiskApplication(
            string fname,
            string lname,
            string col1,
            string col2,
            string col3,
            string col4,
            string col5,
            string col6,
            string col7,
            string col8,
            string col9,
            string col10,
            string col11,
            string col12,
            string col13,
            string col14,
            string col15,
            string col16,
            string col17,
            string col18,
            string col19,
            string col20,
            string col21 )
        {
            FirstName = fname;
            LastName = lname;
            CheckingAccountStatus = col1;
            DurationInMonths = col2;
            CreditHistory = col3;
            Purpose = col4;
            CreditAmount = col5;
            SavingsAccount = col6;
            PresentEmploymentSince = col7;
            InstallmentPecentageOfDI = col8;
            PersonalStatusAndSex = col9;
            OtherDebtors = col10;
            PresentResidenceSince = col11;
            Property = col12;
            AgeInYears = col13;
            OtherInstallments = col14;
            Housing = col15;
            NumberOfExistingCredits = col16;
            Job = col17;
            NumberOfPeopleLiable = col18;
            Telephone = col19;
            ForeignWorker = col20;
            CreditRisk = col21;
        }

        [DisplayName("First Name")]
        public string FirstName { get; set; }
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [DisplayName("Checking Account Status")]
        public string CheckingAccountStatus { get; set; }
        public string DurationInMonths { get; set; }
        public string CreditHistory { get; set; }
        public string Purpose { get; set; }
        public string CreditAmount { get; set; }
        public string SavingsAccount { get; set; }
        public string PresentEmploymentSince { get; set; }
        public string InstallmentPecentageOfDI { get; set; }
        public string PersonalStatusAndSex { get; set; }
        public string OtherDebtors { get; set; }
        public string PresentResidenceSince { get; set; }
        public string Property { get; set; }
        public string AgeInYears { get; set; }
        public string OtherInstallments { get; set; }
        public string Housing { get; set; }
        public string NumberOfExistingCredits { get; set; }
        public string Job { get; set; }
        public string NumberOfPeopleLiable { get; set; }
        public string Telephone { get; set; }
        public string ForeignWorker { get; set; }
        public string CreditRisk { get; set; }

        public string ScoredLabels { get; set; }

        public string ScoredProbablities { get; set; }


        public string Message { get; set; }

    }

    public class Rootobject
    {
        public Results Results { get; set; }
    }

    public class Results
    {
        public Output1 output1 { get; set; }
    }

    public class Output1
    {
        public string type { get; set; }
        public Value value { get; set; }
    }

    public class Value
    {
        public string[] ColumnNames { get; set; }
        public string[] ColumnTypes { get; set; }
        public string[][] Values { get; set; }
    }
}
