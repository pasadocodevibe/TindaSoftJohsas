-- Index recommendations based on ADO queries in:
-- SalesRecord.aspx.cs, pos1.aspx.cs, ManageStock.aspx.cs, mStockSheet.aspx.cs
-- Target: SQL Server 2016
-- Review and adjust for existing PK/unique indexes before applying.

-- ========================
-- trans_invoice
-- ========================
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_trans_invoice_set_void_date' AND object_id = OBJECT_ID('dbo.trans_invoice'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_trans_invoice_set_void_date]
    ON dbo.trans_invoice (invoicesetid, invoicevoid, invoicedate)
    INCLUDE (invoiceid, invoiceno, invoicecustomer, invoicesubtotal, invoicediscountamt, invoicetax, invoicetotal, invoinceamounttendered, invoicechanged, invoicenote, invoicentryby);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_trans_invoice_id_set_void' AND object_id = OBJECT_ID('dbo.trans_invoice'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_trans_invoice_id_set_void]
    ON dbo.trans_invoice (invoiceid, invoicesetid, invoicevoid)
    INCLUDE (invoicedate, invoicecustomerid, invoicetotal, invoicediscountamt, invoicetax, invoicesubtotal, invoicenote);
END
GO

-- ========================
-- trans_invoicecart
-- ========================
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_trans_invoicecart_set_void_invoice_date' AND object_id = OBJECT_ID('dbo.trans_invoicecart'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_trans_invoicecart_set_void_invoice_date]
    ON dbo.trans_invoicecart (cartsetid, cartvoid, cartinvoiceid, cartdatecreated)
    INCLUDE (cartid, cartproductid, cartprice, cartqty, cartamount, cartstatus, cartinvoiceid);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_trans_invoicecart_invoice_void' AND object_id = OBJECT_ID('dbo.trans_invoicecart'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_trans_invoicecart_invoice_void]
    ON dbo.trans_invoicecart (cartinvoiceid, cartvoid)
    INCLUDE (cartid, cartproductid, cartqty, cartamount, cartdatecreated, cartsetid);
END
GO

-- ========================
-- trans_inventory
-- ========================
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_trans_inventory_set_void_prod_date' AND object_id = OBJECT_ID('dbo.trans_inventory'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_trans_inventory_set_void_prod_date]
    ON dbo.trans_inventory (inventorysetid, inventoryvoid, inproductid, inventorydate)
    INCLUDE (inventoryqty, inventorytype, inventorynote, inventoryentryby, invent_lotno, invent_expirydate, inventorycostperunit, invent_soldsalesid, invent_soldcartid);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_trans_inventory_prod_void_type' AND object_id = OBJECT_ID('dbo.trans_inventory'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_trans_inventory_prod_void_type]
    ON dbo.trans_inventory (inproductid, inventoryvoid, inventorytype, inventorysetid)
    INCLUDE (inventoryqty, inventorydate, invent_lotno, invent_expirydate);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_trans_inventory_sales_cart' AND object_id = OBJECT_ID('dbo.trans_inventory'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_trans_inventory_sales_cart]
    ON dbo.trans_inventory (invent_soldsalesid, invent_soldcartid)
    INCLUDE (inventoryid, inventoryvoid, inventorysetid, inventoryqty, inventorycostperunit);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_trans_inventory_set_void_product_id' AND object_id = OBJECT_ID('dbo.trans_inventory'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_trans_inventory_set_void_product_id]
    ON dbo.trans_inventory (inventorysetid, inventoryvoid, inproductid, inventoryid)
    INCLUDE (inventoryqty, inventorytype, inventorydate);
END
GO

-- ========================
-- trans_product
-- ========================
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_trans_product_set_type_status' AND object_id = OBJECT_ID('dbo.trans_product'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_trans_product_set_type_status]
    ON dbo.trans_product (prodsetid, producttype, prodstatus, prodvoid)
    INCLUDE (productid, productname, prodsellprice, prodbrand, productunit, prodbaseunit, productcateg, prodreorderlevel, prodbarcode);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_trans_product_set_status_barcode' AND object_id = OBJECT_ID('dbo.trans_product'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_trans_product_set_status_barcode]
    ON dbo.trans_product (prodsetid, prodstatus, prodvoid, prodbarcode)
    INCLUDE (productid, productname, prodsellprice, prodbrand, productunit, prodbaseunit);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_trans_product_set_categ_name' AND object_id = OBJECT_ID('dbo.trans_product'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_trans_product_set_categ_name]
    ON dbo.trans_product (prodsetid, prodvoid, producttype, productcateg, productname)
    INCLUDE (productid, prodbaseunit, productunit, prodstatus);
END
GO

-- ========================
-- trans_customer
-- ========================
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_trans_customer_set_status' AND object_id = OBJECT_ID('dbo.trans_customer'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_trans_customer_set_status]
    ON dbo.trans_customer (csetid, cstatus, cvoid)
    INCLUDE (customerid, cfullname, refno, caddress, ccontact, cemail);
END
GO

-- ========================
-- ref_account
-- ========================
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ref_account_usname' AND object_id = OBJECT_ID('dbo.ref_account'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_ref_account_usname]
    ON dbo.ref_account (usname)
    INCLUDE (uid, ufullname, usetid, ustatus, uvoid);
END
GO

-- ========================
-- ref_generalsettings
-- ========================
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ref_generalsettings_setid' AND object_id = OBJECT_ID('dbo.ref_generalsettings'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_ref_generalsettings_setid]
    ON dbo.ref_generalsettings (setid)
    INCLUDE (setcompanyname, setaddress, setcontact, setvoid);
END
GO

-- ========================
-- ref_units / ref_productcategory / ref_brand / ref_identification
-- (small lookup tables; indexes only if not already PK/unique)
-- ========================
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ref_units_uomid' AND object_id = OBJECT_ID('dbo.ref_units'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_ref_units_uomid]
    ON dbo.ref_units (uomid)
    INCLUDE (uomname, uomvoid);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ref_productcategory_setid_status' AND object_id = OBJECT_ID('dbo.ref_productcategory'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_ref_productcategory_setid_status]
    ON dbo.ref_productcategory (pcategsetid, pcategvoid, pcategstatus)
    INCLUDE (pcategid, pcategname);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ref_brand_id' AND object_id = OBJECT_ID('dbo.ref_brand'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_ref_brand_id]
    ON dbo.ref_brand (id)
    INCLUDE (name);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ref_identification_setid_void' AND object_id = OBJECT_ID('dbo.ref_identification'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_ref_identification_setid_void]
    ON dbo.ref_identification (setid, void)
    INCLUDE (name);
END
GO

