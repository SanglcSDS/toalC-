using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filewatcherservice
{
    public class TransactionDetail
    {
        private string cashRequest;
        private string cashPersented;
        private string transNo;
        private string segNo;
        private string dateTime;
        private string txnType;
        private string cardNo;
        private string amount;
        private string cashTaken;

        public TransactionDetail()
        {

        }
        public TransactionDetail(string cashRequest)
        {
            this.cashRequest = cashRequest;
        }

        public void setCashRequest(string value)
        {
            cashRequest = value;
        }

        public string getCashRequest()
        {
            return cashRequest;
        }
        public void setCashPersented(string value)
        {
            cashPersented = value;
        }

        public string getCashPersented()
        {
            return cashPersented;
        }
        public void setTransNo(string value)
        {
            transNo = value;
        }

        public string getTransNo()
        {
            return transNo;
        }
        public void setSegNo(string value)
        {
            segNo = value;
        }

        public string getSegNo()
        {
            return segNo;
        }
        public void setDateTime(string value)
        {
            dateTime = value;
        }

        public string getDateTime()
        {
            return dateTime;
        }
        public void setTxnType(string value)
        {
            txnType = value;
        }

        public string getTxnType()
        {
            return txnType;
        }
        public void setCardNo(string value)
        {
            cardNo = value;
        }

        public string getCardNo()
        {
            return cardNo;
        }
        public void setAmount(string value)
        {
            amount = value;
        }

        public string getAmount()
        {
            return amount;
        }
        public void setCashTaken(string value)
        {
            cashTaken = value;
        }

        public string getCashTaken()
        {
            return cashTaken;
        }

    }
}
