="Mapping PO - SPA"	=""	=""	=""	=""	=""	=""	=""	=""	=""	=""
=""	=""	=""	=""	=""	=""	="SPA"	=""	=""	=""	=""
="Columns"	="Name Query"	="Data_Type"	="Table"	="RelashionShip"	="Comments"	="Data"	="FieldsName"	="Datatype"	="Staging table"	="SPA Comments"
="[No.] (PO Number (with revision)) "	="PoReferenceRevised"	="varchar(30)"	="[SBM$COSL Purchase Header] (POH)"	=""	=""	="PO Number (with revision)"	="sbm_ponumber"	="Text(100)"	="sbm_stagedpurchaseorder"	=""
="MDM ID (MDM VendorID)"	="[Identity],"	="varchar(30)"	="[SBM$Vendor]  ( V )"	="POH.[Buy-from Vendor No.] = V.[No.] AND V.[MDM ID] != ''"	=""	="MDM VendorID"	="sbm_mdmnumber"	="Text(100)"	="sbm_stagedpurchaseorder"	=""
="Major Package Manager GUID (PKM PersonID)"	="In Lucy from Email"	="varchar(40)"	="[SBM$Package] (PK) / [SBM$Resource] R"	="R.[No.] = PK.[Major Package Manager GUID]"	=""	="PKM PersonID"	="sbm_pkmpersonid"	="Text(100)"	="sbm_stagedpurchaseorder"	="Expects a Guid(ObjectId), wihch matches a user in Entra Id (UPN)"
="Email (PKM Email)"	="Email"	="varchar(80)"	="[SBM$Resource] R"	=""	="Retrieve email from Resource [SBM$Resource]"	="PKM Email"	="sbm_pkmemail"	="Text(100)"	="sbm_stagedpurchaseorder"	=""
="Product Id (Category/Equipment Code)"	="[Product Id]"	="varchar(20) (XXX.XXX.XXX.XXX)"	="[SBM$Package] (PK)"	="POH.[Procurement Package No.] = PK.[Procurement Package No.] 
            AND POH.[Job No.] = PK.[Project Id] 
            AND POH.[Suffix] = PK.[Suffix]"	=""	="Product Code"	="sbm_productcode"	="Text(100)"	="sbm_stagedpurchaseorder"	=""
="Total PO Value (FCY) (Amount)"	="PoCurrAmt"	="decimal(38,20)"	="[SBM$Purchase Header Extension]  PHE"	="POH.[No.] = phe.[No.]  AND PHE.[Document Type] ='1'"	=""	="Amount"	="sbm_amount"	="Decimal"	="sbm_stagedpurchaseorder"	="Min: -100.000.00.000. Max: 100.000.000.000. Decimal places: 2"
="Actual Supplier Delivery Date (First Delivery Date)"	="FirstDeliveryDate"	="datetime"	="[SBM$Package Lines]"	="POH.[Procurement Package No.] = PKL.[Procurement Package Number] 
            AND POH.[Job No.] = PKL.[Project Id] 
            AND PKL.[Procurement Package Suffix] = POH.[Suffix]"	="It is possible to transfer only the date and not the date and time."	="First Delivery Date"	="sbm_firstdelivery"	="Dateonly"	="sbm_stagedpurchaseorder"	="Is the currency always USD?"
="Actual Supplier Delivery Date (Last Delivery Date)"	="LastDeliveryDate"	="datetime"	="[SBM$Package Lines]"	="POH.[Procurement Package No.] = PKL.[Procurement Package Number] 
            AND POH.[Job No.] = PKL.[Project Id] 
            AND PKL.[Procurement Package Suffix] = POH.[Suffix]"	="It is possible to transfer only the date and not the date and time."	="Last Delivery Date"	="sbm_lastdelivery"	="Dateonly"	="sbm_stagedpurchaseorder"	=""
="PO Close Out Boolean"	="PO Close Out Boolean"	="Boolean (Caculated)
Last Delivery Date + 6 months"	="[SBM$Package Lines]"	="POH.[Procurement Package No.] = PKL.[Procurement Package Number] 
            AND POH.[Job No.] = PKL.[Project Id] 
            AND PKL.[Procurement Package Suffix] = POH.[Suffix]"	="Calculated : Last Delivery Date + 6 months"	="PO Close Out Boolean"	="sbm_closeout"	="Boolean"	="sbm_stagedpurchaseorder"	=""
="Last Modified Date  + ' ' + Modified Time (Last Update Date)"	="DateModified"	="datetime2(3) (YYYY-MM-DD hh:mm:ss.fff)"	="[SBM$Package Lines]"	=""	="Calculated : Last modified date / Modified Time on Package Lines"	="Last Update Date"	="sbm_erplastupdate"	="Date & Time"	="sbm_stagedpurchaseorder"	=""
="[Job No.]  (Project Number)"	="ProjectId"	="varchar(20)"	="SBM$COSL Purchase Header (POH)"	=""	=""	="Project Number"	="sbm_projectnumber"	="Text(100)"	="sbm_stagedpurchaseorder"	=""
="Description "	="Description"	="varchar(100)"	="[SBM$Package] (PK)"	="POH.[Procurement Package No.] = PK.[Procurement Package No.] 
            AND POH.[Job No.] = PK.[Project Id] 
            AND POH.[Suffix] = PK.[Suffix]"	=""	="Description "	="sbm_description"	="Text(2000)"	="sbm_stagedpurchaseorder"	=""
=""	=""	=""	=""	=""	=""	=""	="sbm_pkmfirstname "	="Text(100)"	="sbm_stagedpurchaseorder"	="Expects 'givenName' from Entra User"
=""	=""	=""	=""	=""	=""	=""	="sbm_pkmlastname"	="Text(100)"	="sbm_stagedpurchaseorder"	="Expects 'surName' from Entra User"
=""	=""	=""	=""	=""	=""	=""	="statuscode"	="Choice"	="sbm_stagedpurchaseorder"	="Values: 1 (Draft), 918860002 (Ready to be Processed). NB: This value is different from the one used on other Staging tables!"
=""	=""	=""	=""	=""	=""	=""	=""	=""	=""	=""
="Stored Procedure "	=""	=""	=""	=""	=""	=""	=""	=""	=""	=""
="Filter Applied :"	=""	=""	=""	=""	=""	=""	=""	=""	=""	=""
="Last Execution Date"	=""	=""	=""	=""	=""	=""	=""	=""	=""	=""
="Amount Above 100K"	=""	=""	=""	=""	=""	=""	=""	=""	=""	=""
="ProductCode that contains in three first characters in LIST : PKG,EQT, BLK,SER, and LOG"	=""	=""	=""	=""	=""	=""	=""	=""	=""	=""
="Exclude service-related POs (SER) and Logistics LOG from shipment if PO close out false"	=""	=""	=""	=""	=""	=""	=""	=""	=""	=""
=""	=""	=""	=""	=""	=""	=""	=""	=""	=""	=""