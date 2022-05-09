import React, {useContext} from 'react';
import {AppBar, Button, Container, IconButton, Toolbar, Typography} from "@mui/material";
import MenuIcon from '@mui/icons-material/Menu';
import './layout.css';
import LoginDialog from "../LoginDialog";
import {UserContext} from "../../context/UserContext";


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