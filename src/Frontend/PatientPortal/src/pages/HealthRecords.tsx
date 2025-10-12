import React from 'react';
import { Typography, Box } from '@mui/material';

const HealthRecords: React.FC = () => {
  return (
    <Box>
      <Typography variant="h4" gutterBottom fontWeight="bold">
        Health Records
      </Typography>
      <Typography variant="body1" color="text.secondary">
        View and manage medical history, allergies, medications, and immunizations.
      </Typography>
      {/* TODO: Implement health records UI */}
    </Box>
  );
};

export default HealthRecords;
