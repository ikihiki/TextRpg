import React from 'react';

interface Note {
  id: string;
  title: string;
  content: string;
  type: string;
  isPinned: boolean;
}

interface NotesPanelProps {
  notes?: Note[];
  onNoteSelect?: (note: Note) => void;
}

/**
 * NotesPanel component for displaying canonical notes (Lorebook).
 */
export const NotesPanel: React.FC<NotesPanelProps> = ({
  notes = [],
  onNoteSelect,
}) => {
  const pinnedNotes = notes.filter((n) => n.isPinned);
  const otherNotes = notes.filter((n) => !n.isPinned);

  return (
    <div className="notes-panel">
      <h3>Canonical Notes</h3>

      {pinnedNotes.length > 0 && (
        <div className="pinned-notes">
          <h4>📌 Pinned</h4>
          <ul>
            {pinnedNotes.map((note) => (
              <li key={note.id} onClick={() => onNoteSelect?.(note)}>
                <strong>{note.title}</strong>
                <span className="note-type">{note.type}</span>
              </li>
            ))}
          </ul>
        </div>
      )}

      <div className="other-notes">
        <ul>
          {otherNotes.map((note) => (
            <li key={note.id} onClick={() => onNoteSelect?.(note)}>
              <strong>{note.title}</strong>
              <span className="note-type">{note.type}</span>
            </li>
          ))}
        </ul>
      </div>
    </div>
  );
};

export default NotesPanel;
