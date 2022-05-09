import React, {useContext} from 'react';
import {AppBar, Button, Container, IconButton, Toolbar, Typography} from "@mui/material";
import './layout.css';
import {UserContext} from "../../context/UserContext";
import {navigate} from "gatsby"
import LoginDialog from "../LoginDialog";
import {Home} from "@mui/icons-material";

interface LayoutProps {
  children: React.ReactChild,
}

const Layout = ({children}: LayoutProps) => {
  const [open, setOpen] = React.useState(false);
  const auth = useContext(UserContext);
  return (
    <>
      <AppBar position="static">
        <Toolbar>
          <Button
            color="inherit" onClick={() => navigate('/')}
            startIcon={<Home />}
          >
            Home
          </Button>
          <Typography variant="h6" component="div" sx={{ml: 2, flexGrow: 1}}>
            Trip Booker
          </Typography>
          <Typography variant="h6" component="div">
            {auth.user.username}
          </Typography>
          {auth.user.username ?
            <Button color="inherit" onClick={() => auth.setUser({username: '', password: ''})}>Log out</Button>
            : <Button color="inherit" onClick={() => setOpen(!open)}>Login</Button>}
        </Toolbar>
      </AppBar>
      <Container>
        {children}
      </Container>
      <LoginDialog open={open} setOpen={setOpen} />
    </>
  );
};

export default Layout;