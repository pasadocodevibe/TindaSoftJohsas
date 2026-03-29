-- Missing index suggestions from SQL Server DMVs
-- Review carefully before applying in production

-- trans_product: product counts and low stock queries
CREATE NONCLUSTERED INDEX IX_trans_product_set_type_status
ON dbo.trans_product (prodsetid, producttype, prodvoid, prodstatus)
INCLUDE (productid, prodreorderlevel, productunit, productname);

-- trans_customer: active customer count
CREATE NONCLUSTERED INDEX IX_trans_customer_set_status
ON dbo.trans_customer (csetid, cvoid, cstatus)
INCLUDE (customerid);

-- ref_account: active user count
CREATE NONCLUSTERED INDEX IX_ref_account_set_status
ON dbo.ref_account (usetid, uvoid, ustatus)
INCLUDE (uid);

-- trans_expenses: sums and top expenses
CREATE NONCLUSTERED INDEX IX_trans_expenses_set_date
ON dbo.trans_expenses (expsetid, expvoid, expdate, expensecateg)
INCLUDE (expamount);

-- trans_invoice: daily/monthly totals and counts
CREATE NONCLUSTERED INDEX IX_trans_invoice_set_date
ON dbo.trans_invoice (invoicesetid, invoicevoid, invoicedate)
INCLUDE (invoicetotal, invoicediscountamt);

-- trans_invoicecart: daily item counts + top sold
CREATE NONCLUSTERED INDEX IX_trans_invoicecart_set_date
ON dbo.trans_invoicecart (cartsetid, cartvoid, cartstatus, cartdatecreated)
INCLUDE (cartqty, cartproductid, cartinvoiceid, cartid);

-- trans_inventory: stock and cost lookups
CREATE NONCLUSTERED INDEX IX_trans_inventory_set_product
ON dbo.trans_inventory (inventorysetid, inventoryvoid, inproductid)
INCLUDE (inventoryqty, inventorycostperunit, invent_soldcartid);
