# DAX Calculations – Bank Transaction Fraud Detection Dashboard

This document outlines the key DAX measures used to build the fraud monitoring dashboard in Power BI.

---

## Key Measures

```DAX
AverageFraudAmount = 
CALCULATE(
    AVERAGE(bank_transaction_fraud_detection[Transaction_Amount]),
    bank_transaction_fraud_detection[Is_Fraud] = 1
)

FraudRate = 
DIVIDE([TotalFraudCases], [TotalTransactions]) * 100

TotalFraudCases = 
CALCULATE(
    COUNTROWS(bank_transaction_fraud_detection),
    bank_transaction_fraud_detection[Is_Fraud] = 1
)

TotalFraudAmount = 
CALCULATE(
    SUM(bank_transaction_fraud_detection[Transaction_Amount]),
    bank_transaction_fraud_detection[Is_Fraud] = 1
)
