import React from 'react';
import { Typography, Box } from '@mui/material';

const Consent: React.FC = () => {
  return (
    <Box>
      <Typography variant="h4" gutterBottom fontWeight="bold">
        Consent & Sharing
      </Typography>
      <Typography variant="body1" color="text.secondary">
        Manage data sharing consents, generate QR codes, and view sharing history.
      </Typography>
      {/* TODO: Implement consent management and QR code generation UI */}
    </Box>
  );
};

export default Consent;
