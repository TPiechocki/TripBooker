import React from 'react';
import {AppBar, Button, Container, IconButton, Toolbar, Typography} from "@mui/material";
import MenuIcon from '@mui/icons-material/Menu';
import './layout.css';


interface LayoutProps {
  children: React.ReactChild,
}

const Layout = ({children}: LayoutProps) => {
  return (
    <>
      <AppBar position="static">
        <Toolbar>
          <IconButton
            size="large"
            edge="start"
            color="inherit"
            aria-label="menu"
            sx={{mr: 2}}
          >
            <MenuIcon/>
          </IconButton>
          <Typography variant="h6" component="div" sx={{flexGrow: 1}}>
            Trip Booker
          </Typography>
          <Button color="inherit">Login</Button>
        </Toolbar>
      </AppBar>
      <Container>
        {children}
      </Container>
    </>
  );
};

export default Layout;