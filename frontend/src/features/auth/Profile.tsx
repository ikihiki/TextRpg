import { useState } from 'react'

interface ProfileProps {
  userId: string
  email: string
  displayName: string
  iconUrl?: string
  bio?: string
  language: string
  onUpdateProfile: (displayName: string, bio: string, language: string) => void
  onLogout: () => void
  onDeleteAccount: () => void
}

export function Profile({
  userId,
  email,
  displayName,
  iconUrl,
  bio,
  language,
  onUpdateProfile,
  onLogout,
  onDeleteAccount
}: ProfileProps) {
  const [editing, setEditing] = useState(false)
  const [editDisplayName, setEditDisplayName] = useState(displayName)
  const [editBio, setEditBio] = useState(bio || '')
  const [editLanguage, setEditLanguage] = useState(language)
  const [showDeleteConfirm, setShowDeleteConfirm] = useState(false)

  const handleSave = () => {
    onUpdateProfile(editDisplayName, editBio, editLanguage)
    setEditing(false)
  }

  const handleCancel = () => {
    setEditDisplayName(displayName)
    setEditBio(bio || '')
    setEditLanguage(language)
    setEditing(false)
  }

  return (
    <div className="profile">
      <div className="profile-header">
        <div className="profile-avatar">
          {iconUrl ? (
            <img src={iconUrl} alt={displayName} />
          ) : (
            <div className="avatar-placeholder">{displayName.charAt(0).toUpperCase()}</div>
          )}
        </div>
        <div className="profile-info">
          <h2>{displayName}</h2>
          <p className="profile-email">{email}</p>
        </div>
      </div>

      {!editing ? (
        <div className="profile-details">
          <div className="detail-row">
            <span className="label">ユーザーID:</span>
            <span className="value">{userId}</span>
          </div>
          <div className="detail-row">
            <span className="label">自己紹介:</span>
            <span className="value">{bio || '未設定'}</span>
          </div>
          <div className="detail-row">
            <span className="label">言語:</span>
            <span className="value">{language}</span>
          </div>
          <div className="profile-actions">
            <button onClick={() => setEditing(true)} className="secondary-button">
              編集
            </button>
            <button onClick={onLogout} className="secondary-button">
              ログアウト
            </button>
          </div>
        </div>
      ) : (
        <div className="profile-edit">
          <div className="form-group">
            <label htmlFor="displayName">表示名</label>
            <input
              id="displayName"
              type="text"
              value={editDisplayName}
              onChange={(e) => setEditDisplayName(e.target.value)}
            />
          </div>
          <div className="form-group">
            <label htmlFor="bio">自己紹介</label>
            <textarea
              id="bio"
              value={editBio}
              onChange={(e) => setEditBio(e.target.value)}
              rows={4}
            />
          </div>
          <div className="form-group">
            <label htmlFor="language">言語</label>
            <select
              id="language"
              value={editLanguage}
              onChange={(e) => setEditLanguage(e.target.value)}
            >
              <option value="ja">日本語</option>
              <option value="en">English</option>
            </select>
          </div>
          <div className="profile-actions">
            <button onClick={handleSave} className="primary-button">
              保存
            </button>
            <button onClick={handleCancel} className="secondary-button">
              キャンセル
            </button>
          </div>
        </div>
      )}

      <div className="danger-zone">
        <h3>危険な操作</h3>
        {!showDeleteConfirm ? (
          <button onClick={() => setShowDeleteConfirm(true)} className="danger-button">
            アカウントを削除
          </button>
        ) : (
          <div className="delete-confirm">
            <p>本当にアカウントを削除しますか？この操作は取り消せません。</p>
            <div className="confirm-actions">
              <button onClick={onDeleteAccount} className="danger-button">
                削除する
              </button>
              <button onClick={() => setShowDeleteConfirm(false)} className="secondary-button">
                キャンセル
              </button>
            </div>
          </div>
        )}
      </div>
    </div>
  )
}
