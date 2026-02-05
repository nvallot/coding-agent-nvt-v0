---
applyTo: ".github/workflows/**"
---

# Instructions CI/CD

Quand tu travailles sur les workflows GitHub Actions:

- Jobs parallèles quand possible
- Caching des dépendances
- Matrix strategy pour multi-versions
- Secrets via GitHub Secrets

```yaml
name: CI

on:
  push:
    branches: [main, develop]
  pull_request:

jobs:
  lint-and-test:
    runs-on: ubuntu-latest
    strategy:
      matrix:
        node-version: [18.x, 20.x]
    
    steps:
      - uses: actions/checkout@v4
      
      - name: Setup Node.js ${{ matrix.node-version }}
        uses: actions/setup-node@v4
        with:
          node-version: ${{ matrix.node-version }}
          cache: 'npm'
      
      - name: Install dependencies
        run: npm ci
      
      - name: Lint
        run: npm run lint
      
      - name: Test
        run: npm test
      
      - name: Upload coverage
        if: matrix.node-version == '20.x'
        uses: codecov/codecov-action@v3
```