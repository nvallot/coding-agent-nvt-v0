---
applyTo: "src/frontend/**"
---

# Instructions Frontend

Quand tu travailles dans le dossier frontend:

- Utiliser React avec TypeScript
- Composants fonctionnels avec Hooks
- Styled Components ou CSS Modules
- Tests avec React Testing Library
- Accessibilité (WCAG 2.1 AA minimum)

```typescript
// Structure de composant
import { FC, useState, useEffect } from 'react';
import styles from './MyComponent.module.css';

interface MyComponentProps {
  title: string;
  onAction: (data: ActionData) => void;
}

export const MyComponent: FC<MyComponentProps> = ({ title, onAction }) => {
  const [data, setData] = useState<Data | null>(null);
  
  useEffect(() => {
    // Side effects
  }, []);
  
  return (
    <div className={styles.container}>
      <h1>{title}</h1>
      {/* Component content */}
    </div>
  );
};
```
