import React from 'react';

interface IllustrationProps {
  imageUrl?: string;
  caption?: string;
  isLoading?: boolean;
}

/**
 * Illustration component for displaying generated illustrations.
 */
export const Illustration: React.FC<IllustrationProps> = ({
  imageUrl,
  caption,
  isLoading = false,
}) => {
  if (isLoading) {
    return (
      <div className="illustration illustration--loading">
        <div className="illustration__placeholder">
          <span>Generating illustration...</span>
        </div>
      </div>
    );
  }

  if (!imageUrl) {
    return null;
  }

  return (
    <figure className="illustration">
      <img src={imageUrl} alt={caption || 'Story illustration'} />
      {caption && <figcaption>{caption}</figcaption>}
    </figure>
  );
};

export default Illustration;
