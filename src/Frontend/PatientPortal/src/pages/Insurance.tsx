import React from 'react';
import { Typography, Box } from '@mui/material';

const Insurance: React.FC = () => {
  return (
    <Box>
      <Typography variant="h4" gutterBottom fontWeight="bold">
        Insurance
      </Typography>
      <Typography variant="body1" color="text.secondary">
        Manage insurance policies, claims, and coverage information.
      </Typography>
      {/* TODO: Implement insurance management UI */}
    </Box>
  );
};

export default Insurance;
