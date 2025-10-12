import React from 'react';
import {
  Grid,
  Card,
  CardContent,
  Typography,
  Box,
  Button,
  Avatar,
  Chip,
  LinearProgress,
  List,
  ListItem,
  ListItemText,
  ListItemAvatar,
  IconButton,
} from '@mui/material';
import {
  TrendingUp as TrendingUpIcon,
  People as PeopleIcon,
  EventAvailable as AppointmentIcon,
  LocalHospital as InsuranceIcon,
  QrCode2 as QRCodeIcon,
  Add as AddIcon,
  Visibility as ViewIcon,
} from '@mui/icons-material';

interface StatCard {
  title: string;
  value: string | number;
  subtitle: string;
  icon: JSX.Element;
  color: string;
  trend?: string;
}

const Dashboard: React.FC = () => {
  const stats: StatCard[] = [
    {
      title: 'Insurance Coverage',
      value: '$7,200',
      subtitle: 'Remaining lifetime coverage',
      icon: <InsuranceIcon />,
      color: '#1976d2',
      trend: '$3,500 until deductible met',
    },
    {
      title: 'Family Members',
      value: '4',
      subtitle: 'Active profiles managed',
      icon: <PeopleIcon />,
      color: '#2e7d32',
    },
    {
      title: 'Upcoming Visits',
      value: '2',
      subtitle: 'In the next 30 days',
      icon: <AppointmentIcon />,
      color: '#ed6c02',
    },
    {
      title: 'Recent Activity',
      value: '12',
      subtitle: 'Actions in the last 30 days',
      icon: <TrendingUpIcon />,
      color: '#9c27b0',
    },
  ];

  const familyMembers = [
    { name: 'John Smith', relation: 'Self', age: 38, status: 'healthy', upcoming: null },
    { name: 'Jane Smith', relation: 'Spouse', age: 36, status: 'healthy', upcoming: 'Physical on Oct 22' },
    { name: 'Emma Smith', relation: 'Daughter', age: 8, status: 'healthy', upcoming: null },
    { name: 'Oliver Smith', relation: 'Son', age: 5, status: 'asthma', upcoming: 'Checkup on Nov 5' },
  ];

  const recentActivity = [
    { action: 'Sent allergy list to St. Mary\'s Hospital', time: '2 hours ago', type: 'share' },
    { action: 'Downloaded lab report', time: '1 day ago', type: 'download' },
    { action: 'Updated Emma\'s vaccination record', time: '3 days ago', type: 'update' },
    { action: 'Added new insurance policy', time: '5 days ago', type: 'create' },
  ];

  return (
    <Box>
      {/* Page Header */}
      <Box sx={{ mb: 4 }}>
        <Typography variant="h4" gutterBottom fontWeight="bold">
          Welcome back, John! 👋
        </Typography>
        <Typography variant="body1" color="text.secondary">
          Here's an overview of your family's health information
        </Typography>
      </Box>

      {/* Quick Stats */}
      <Grid container spacing={3} sx={{ mb: 4 }}>
        {stats.map((stat, index) => (
          <Grid item xs={12} sm={6} md={3} key={index}>
            <Card sx={{ height: '100%' }}>
              <CardContent>
                <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                  <Avatar sx={{ bgcolor: stat.color, mr: 2 }}>
                    {stat.icon}
                  </Avatar>
                  <Typography variant="h4" fontWeight="bold">
                    {stat.value}
                  </Typography>
                </Box>
                <Typography variant="body2" color="text.secondary" gutterBottom>
                  {stat.title}
                </Typography>
                <Typography variant="caption" color="text.secondary">
                  {stat.subtitle}
                </Typography>
                {stat.trend && (
                  <Box sx={{ mt: 1 }}>
                    <LinearProgress variant="determinate" value={65} sx={{ height: 6, borderRadius: 3 }} />
                    <Typography variant="caption" color="primary" sx={{ mt: 0.5, display: 'block' }}>
                      {stat.trend}
                    </Typography>
                  </Box>
                )}
              </CardContent>
            </Card>
          </Grid>
        ))}
      </Grid>

      {/* Quick Actions */}
      <Box sx={{ mb: 4 }}>
        <Typography variant="h6" gutterBottom fontWeight="bold">
          Quick Actions
        </Typography>
        <Grid container spacing={2}>
          <Grid item>
            <Button
              variant="contained"
              size="large"
              startIcon={<QRCodeIcon />}
              sx={{ borderRadius: 2 }}
            >
              Generate QR Code
            </Button>
          </Grid>
          <Grid item>
            <Button
              variant="outlined"
              size="large"
              startIcon={<AddIcon />}
              sx={{ borderRadius: 2 }}
            >
              Add Family Member
            </Button>
          </Grid>
          <Grid item>
            <Button
              variant="outlined"
              size="large"
              startIcon={<AddIcon />}
              sx={{ borderRadius: 2 }}
            >
              Upload Document
            </Button>
          </Grid>
        </Grid>
      </Box>

      <Grid container spacing={3}>
        {/* Family Members */}
        <Grid item xs={12} md={6}>
          <Card>
            <CardContent>
              <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
                <Typography variant="h6" fontWeight="bold">
                  Family Members
                </Typography>
                <Button size="small" startIcon={<AddIcon />}>
                  Add
                </Button>
              </Box>
              <List>
                {familyMembers.map((member, index) => (
                  <ListItem
                    key={index}
                    secondaryAction={
                      <IconButton edge="end">
                        <ViewIcon />
                      </IconButton>
                    }
                    sx={{ px: 0 }}
                  >
                    <ListItemAvatar>
                      <Avatar sx={{ bgcolor: 'primary.light' }}>
                        {member.name.charAt(0)}
                      </Avatar>
                    </ListItemAvatar>
                    <ListItemText
                      primary={
                        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                          <Typography variant="body1">{member.name}</Typography>
                          <Chip
                            label={member.status === 'healthy' ? 'Healthy' : 'Asthma'}
                            size="small"
                            color={member.status === 'healthy' ? 'success' : 'warning'}
                          />
                        </Box>
                      }
                      secondary={
                        <>
                          <Typography component="span" variant="body2" color="text.secondary">
                            {member.relation} • {member.age} years old
                          </Typography>
                          {member.upcoming && (
                            <>
                              <br />
                              <Typography component="span" variant="caption" color="primary">
                                {member.upcoming}
                              </Typography>
                            </>
                          )}
                        </>
                      }
                    />
                  </ListItem>
                ))}
              </List>
            </CardContent>
          </Card>
        </Grid>

        {/* Recent Activity */}
        <Grid item xs={12} md={6}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom fontWeight="bold">
                Recent Activity
              </Typography>
              <List>
                {recentActivity.map((activity, index) => (
                  <ListItem key={index} sx={{ px: 0 }}>
                    <ListItemText
                      primary={activity.action}
                      secondary={activity.time}
                    />
                  </ListItem>
                ))}
              </List>
              <Button fullWidth variant="text" sx={{ mt: 1 }}>
                View All Activity
              </Button>
            </CardContent>
          </Card>
        </Grid>

        {/* Upcoming Appointments */}
        <Grid item xs={12}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom fontWeight="bold">
                Upcoming Appointments
              </Typography>
              <Box sx={{ mt: 2 }}>
                <Box sx={{ p: 2, bgcolor: 'primary.light', color: 'white', borderRadius: 2, mb: 2 }}>
                  <Typography variant="subtitle1" fontWeight="bold">
                    Jane Smith - Annual Physical
                  </Typography>
                  <Typography variant="body2">
                    📅 October 22, 2025 at 10:00 AM
                  </Typography>
                  <Typography variant="body2">
                    📍 St. Mary's Hospital - Dr. Johnson
                  </Typography>
                </Box>
                <Box sx={{ p: 2, bgcolor: 'grey.100', borderRadius: 2 }}>
                  <Typography variant="subtitle1" fontWeight="bold">
                    Oliver Smith - Pediatric Checkup
                  </Typography>
                  <Typography variant="body2" color="text.secondary">
                    📅 November 5, 2025 at 2:30 PM
                  </Typography>
                  <Typography variant="body2" color="text.secondary">
                    📍 Children's Clinic - Dr. Martinez
                  </Typography>
                </Box>
              </Box>
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </Box>
  );
};

export default Dashboard;
