import React from 'react';

interface ActionPanelProps {
  actions?: string[];
  onActionSelect?: (action: string) => void;
}

/**
 * ActionPanel component for displaying available player actions.
 */
export const ActionPanel: React.FC<ActionPanelProps> = ({
  actions = [],
  onActionSelect,
}) => {
  return (
    <div className="action-panel">
      <h3>Available Actions</h3>
      <ul>
        {actions.map((action, index) => (
          <li key={index}>
            <button onClick={() => onActionSelect?.(action)}>{action}</button>
          </li>
        ))}
      </ul>
    </div>
  );
};

export default ActionPanel;
