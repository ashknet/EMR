import React from 'react';
import { Typography, Box } from '@mui/material';

const Documents: React.FC = () => {
  return (
    <Box>
      <Typography variant="h4" gutterBottom fontWeight="bold">
        Documents
      </Typography>
      <Typography variant="body1" color="text.secondary">
        Upload, view, and share medical documents securely.
      </Typography>
      {/* TODO: Implement document vault UI */}
    </Box>
  );
};

export default Documents;
