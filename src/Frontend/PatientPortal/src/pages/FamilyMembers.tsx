import React from 'react';
import { Typography, Box } from '@mui/material';

const FamilyMembers: React.FC = () => {
  return (
    <Box>
      <Typography variant="h4" gutterBottom fontWeight="bold">
        Family Members
      </Typography>
      <Typography variant="body1" color="text.secondary">
        Manage your family members and their health records.
      </Typography>
      {/* TODO: Implement family member management UI */}
    </Box>
  );
};

export default FamilyMembers;
