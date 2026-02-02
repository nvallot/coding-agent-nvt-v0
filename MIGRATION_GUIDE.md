# Migration Guide: FAP-57 Refactoring

## ✅ COMPLETED STEPS

The FAP-57 (SendPOSupplier) code has been successfully **migrated to the Supplier Portal repository**.

### What has been created:

**New Supplier Portal Repository:**
```
src/SupplierPortal/
├── SupplierPortal.sln                 ← New solution file
├── FAP-57.SendPOSupplier/             ← Complete function app code
│   ├── SendPOSupplierFunction.cs
│   ├── Program.cs
│   ├── host.json
│   ├── local.settings.json
│   ├── FAP-57.SendPOSupplier.csproj
│   └── Services/
│       ├── ILucyApiService.cs
│       ├── LucyApiService.cs
│       ├── IDataverseService.cs
│       └── DataverseService.cs
├── FAP-57.Tests/                      ← Complete test project
│   ├── SendPOSupplierFunctionTests.cs
│   └── FAP-57.Tests.csproj
├── Shared.Models/                     ← Shared models (local copy)
│   ├── PurchaseOrderMessage.cs
│   ├── DataverseStaging.cs
│   ├── LucyUserResponse.cs
│   └── Shared.Models.csproj
└── README.md                          ← Repository documentation
```

**Updated NADIA Repository:**
```
src/NADIA/
├── NadiaSpaIntegration.sln           ← Removed FAP-57 projects
├── FAP-65.RetrivePOVendor/           ← Only FAP-65 now
├── FAP-65.Tests/
├── Shared.Models/                     ← Local copy maintained
├── monitoring/
├── README.md                          ← Updated with new architecture
└── DEPLOYMENT.md                      ← Still valid for both repos
```

### New Documentation:
- `ARCHITECTURE.md` (root level) - Multi-repository architecture overview
- `src/SupplierPortal/README.md` - Supplier Portal specific documentation
- Updated `src/NADIA/README.md` - References Supplier Portal repo

---

## ⚠️ MANUAL CLEANUP REQUIRED

The old FAP-57 folders **still exist** in the NADIA directory and must be **manually deleted** or cleaned up:

### To Delete (from NADIA folder):
```
❌ src/NADIA/FAP-57.SendPOSupplier/
❌ src/NADIA/FAP-57.Tests/
```

### Files already updated:
```
✅ src/NADIA/NadiaSpaIntegration.sln - Removed FAP-57 project references
✅ src/NADIA/README.md - Updated with new architecture
```

### How to clean up (option 1: via VS Code):
1. Open `src/NADIA/`
2. Right-click on `FAP-57.SendPOSupplier/` folder → Delete
3. Right-click on `FAP-57.Tests/` folder → Delete
4. Commit changes: `git rm -r FAP-57.SendPOSupplier/ FAP-57.Tests/`

### How to clean up (option 2: via terminal):
```bash
cd src/NADIA/
rm -Recurse FAP-57.SendPOSupplier/
rm -Recurse FAP-57.Tests/
git add -A
git commit -m "refactor: Move FAP-57 to Supplier Portal repository"
```

---

## 🔗 Repository Links

### Two separate repositories now:

1. **NADIA Repository** - Data Extraction (FAP-65)
   - Location: `src/NADIA/`
   - Primary file: `NadiaSpaIntegration.sln`
   - Contains: FAP-65, Tests, Shared.Models, Infrastructure config

2. **Supplier Portal Repository** - Data Enrichment & Integration (FAP-57)
   - Location: `src/SupplierPortal/`
   - Primary file: `SupplierPortal.sln`
   - Contains: FAP-57, Tests, Shared.Models (local copy)

### In Azure DevOps CI/CD:
- Separate pipelines per repository
- Triggered independently on code changes
- Can be deployed separately or together

---

## ✨ Benefits of this architecture

1. **Separation of Concerns**
   - FAP-65: Focuses only on data extraction
   - FAP-57: Focuses on enrichment & integration

2. **Independent Scaling**
   - Each Function App can scale independently
   - Different retry/timeout policies per scenario

3. **Team Ownership**
   - Easier to assign teams to each repo
   - Cleaner responsibility boundaries

4. **Deployment Control**
   - FAP-65 can deploy without FAP-57
   - FAP-57 can deploy without FAP-65
   - Reduces blast radius of bugs

5. **Testing Isolation**
   - Each test suite is focused
   - Easier to mock external dependencies

---

## 📋 Verification Checklist

- [x] FAP-57 code copied to SupplierPortal repo
- [x] All services copied (LucyApiService, DataverseService)
- [x] Shared models replicated
- [x] Test projects created
- [x] SupplierPortal.sln created
- [x] NADIA solution file updated (FAP-57 removed)
- [x] README.md files updated
- [x] ARCHITECTURE.md created
- [ ] Manual cleanup of old FAP-57 folders from NADIA
- [ ] Test build: `dotnet build src/NADIA/NadiaSpaIntegration.sln`
- [ ] Test build: `dotnet build src/SupplierPortal/SupplierPortal.sln`
- [ ] Update Azure DevOps pipeline definitions
- [ ] Configure separate deployments in CI/CD

---

## 🚀 Next Steps

1. **Delete old FAP-57 folders** from NADIA directory (see Manual Cleanup section)
2. **Verify builds**:
   ```bash
   cd src/NADIA && dotnet build
   cd src/SupplierPortal && dotnet build
   ```
3. **Create separate Azure DevOps pipelines** (or modify existing one to trigger both repos)
4. **Deploy to DEV environment** and test end-to-end flow
5. **Update team documentation** to reference new multi-repo structure

---

**Last Updated**: Jan 30, 2026  
**Status**: Ready for cleanup and testing ✅
