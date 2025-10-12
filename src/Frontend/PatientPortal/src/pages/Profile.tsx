import React from 'react';
import { Typography, Box } from '@mui/material';

const Profile: React.FC = () => {
  return (
    <Box>
      <Typography variant="h4" gutterBottom fontWeight="bold">
        My Profile
      </Typography>
      <Typography variant="body1" color="text.secondary">
        Manage your personal information and account settings.
      </Typography>
      {/* TODO: Implement profile management UI */}
    </Box>
  );
};

export default Profile;
