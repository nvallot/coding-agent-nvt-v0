# START HERE 👋

Welcome to the GitHub Copilot Multi-Agent System for Azure Data Integration!

## 🎯 What Is This?

This is a specialized AI assistant system built on GitHub Copilot that helps you work on Azure data integration projects. It consists of four AI agents, each with specific expertise:

- **@ba** - Business Analyst (requirements, BRDs)
- **@archi** - Solution Architect (architecture, TADs, decisions)
- **@dev** - Developer (C#, Python, Terraform, data pipelines)
- **@reviewer** - Code Reviewer (quality, security, performance)

## ⚡ Quick Start (5 Minutes)

### 1. Verify Installation ✓

Open **GitHub Copilot Chat** in VS Code (or press `Ctrl+Shift+I` / `Cmd+Shift+I`)

Type `@` and you should see four agents appear:
- @ba
- @archi  
- @dev
- @reviewer

✅ If you see them, you're ready! Skip to step 3.

❌ If not, the files may not be in the right place. Check that `.github/` folder contains:
- `.github/agents/` (4 agent files)
- `.github/config/copilot-config.json`

### 2. Understand Your Current Client

By default, you're using the **"default"** client configuration.

Check which client is active:
```bash
cat .github/clients/active-client.json
```

The default configuration is suitable for general Azure data integration work.

### 3. Try Your First Agent Interaction

Open Copilot Chat and try this:

```
@ba I need to migrate data from an on-premises SQL Server to Azure Synapse Analytics. Can you help me understand the requirements?
```

The Business Analyst agent will ask you clarifying questions and help analyze the need.

### 4. Try a Complete Workflow

Follow this workflow for a simple task:

**Step 1: Requirements**
```
@ba /analyze "Daily sync of customer orders from SQL Server to Azure SQL, need data within 15 minutes"
```

**Step 2: Architecture**
```
@archi /design "Architecture for near real-time data sync from SQL Server to Azure SQL"
```

**Step 3: Implementation**
```
@dev Create an Azure Data Factory pipeline that syncs customer_orders table incrementally using change tracking
```

**Step 4: Review**
```
@reviewer /review "Check this ADF pipeline for best practices"
```

### 5. Explore Available Commands

Each agent has special commands. Try:

- `@ba /brd` - Generate a Business Requirements Document
- `@archi /diagram` - Create architectural diagrams
- `@archi /tad` - Generate a Technical Architecture Document
- `@dev /implement` - Implement a feature
- `@dev /pipeline` - Create a data pipeline
- `@reviewer /security` - Security-focused review

## 📖 What's Available

### 🎓 Skills (Specialized Knowledge)

Your agents can access 7 specialized skills:
- **solution-design** - How to design complete solutions
- **diagram-creation** - Creating C4, UML, infrastructure diagrams
- **code-implementation** - Best practices for writing code
- **code-review** - How to review code effectively
- **testing** - Testing strategies and frameworks
- **debugging** - Debugging techniques
- **security-audit** - Security assessment methods

### 📚 Knowledge Base

Comprehensive documentation on:
- **Azure Services** - Data Factory, Synapse, Databricks, ADLS Gen2, Event Hubs, etc.
- **Architecture Patterns** - Common patterns for data integration
- **Best Practices** - Industry standards, Azure CAF, security, performance

### 📝 Coding Standards (Auto-Applied)

When you work on code, standards are automatically applied based on file type:
- **C# files** (*.cs) → C# conventions, security, performance
- **Python files** (*.py) → PEP 8, type hints, best practices
- **Terraform files** (*.tf) → Azure CAF naming, modules, security
- **Data pipelines** → Parameterization, idempotency, data quality

You don't need to remember these—they're applied automatically!

### 🔄 CI/CD Pipelines

Three ready-to-use pipelines:
1. **.NET Build & Deploy** - For C# applications and APIs
2. **Terraform Deploy** - For infrastructure as code
3. **Azure Data Factory Deploy** - For data pipelines

## 🏢 Working with Multiple Clients

If you work on different client projects, you can create isolated configurations:

### Create a New Client

```bash
./.github/tools/client-manager.sh create contoso
```

This creates a new client folder from the template.

### Edit Client Configuration

```bash
# Edit the CLIENT.md file with client-specific details
nano .github/clients/contoso/CLIENT.md
```

Customize:
- Client name and context
- Azure services they use
- Specific naming conventions
- Security requirements
- Compliance needs (GDPR, HIPAA, etc.)

### Switch to the Client

```bash
./.github/tools/client-manager.sh activate contoso
```

Now when you work with agents, they'll use contoso-specific context!

### List All Clients

```bash
./.github/tools/client-manager.sh list
```

## 💡 Pro Tips

### 1. Be Specific

❌ Bad: "Create a data pipeline"
✅ Good: "Create an ADF pipeline that loads customer_orders from SQL Server to ADLS Gen2 daily at 2 AM, using incremental load based on modified_date"

### 2. Use Commands

Commands give you structured outputs:
- `/analyze` - Structured analysis
- `/design` - Complete architecture
- `/implement` - Full implementation
- `/review` - Comprehensive review

### 3. Ask for Diagrams

```
@archi Create a C4 container diagram showing the data flow from source to destination
```

### 4. Reference Knowledge

```
@archi According to Azure best practices, what's the recommended way to handle secrets in Terraform?
```

### 5. Chain Agents

```
@ba Create user stories for this requirement
@archi Based on these user stories, design the architecture
@dev Implement the solution
@reviewer Review the implementation
```

## 🚨 Common Issues

### Agents Don't Appear

**Solution**: Check that files are in the correct location:
```bash
ls -la .github/agents/
ls -la .github/config/copilot-config.json
```

### Instructions Not Applied

**Solution**: Ensure instruction files have the frontmatter:
```markdown
---
applyTo: "**/*.cs"
---
```

### Client Configuration Not Working

**Solution**: Verify active-client.json points to the right client:
```bash
cat .github/clients/active-client.json
```

## 📚 Next Steps

1. **Read the full README**: `../README.md`
2. **Explore agent capabilities**: `docs/AGENT-USAGE.md`
3. **Set up your client**: `docs/CLIENT-MANAGEMENT.md`
4. **Configure CI/CD**: Review `.github/workflows/` files
5. **Review knowledge base**: Check `.github/knowledge/azure/`

## 🆘 Need Help?

- **Documentation**: Check `/docs` folder
- **Troubleshooting**: `docs/TROUBLESHOOTING.md`
- **Questions**: Ask your agents! They can help explain themselves:
  ```
  @archi What are your main capabilities and when should I use you?
  ```

## 🎉 You're Ready!

You now have a powerful AI assistant system specialized for Azure data integration. Start by analyzing a real requirement from your backlog and watch the agents help you through the entire process.

**Happy building! 🚀**

---

*Remember: The agents are here to assist you, not replace your expertise. Use them to accelerate your work, validate your decisions, and ensure you're following best practices.*
