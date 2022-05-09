import React, {useContext, useState} from 'react';
import {Button, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle, TextField} from "@mui/material";
import {request} from "../api/request";
import {UserContext} from "../context/UserContext";


interface LoginDialogProps {
  open: boolean,
  setOpen: (open: boolean) => void,
}

const LoginDialog = ({open, setOpen}: LoginDialogProps) => {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState(false);

  const auth = useContext(UserContext);

  const checkLogin = () => {
    request('GET', '/Login', undefined, {username, password}).then((data) => {
      auth.setUser({username, password})
      setOpen(false);
    }).catch(() => {setError(true)});
  };

  return (
    <Dialog open={open} onClose={() => setOpen(false)}>
      <DialogTitle>Log in</DialogTitle>
      <DialogContent sx={{'& .MuiFormControl-root': {my: 1}}}>
        <TextField
          autoFocus
          id="name"
          label="Username"
          type="username"
          fullWidth
          value={username}
          onChange={(event) => setUsername(event.target.value)}
          error={error}
        />
        <TextField
          autoFocus
          id="name"
          label="Password"
          type="password"
          fullWidth
          value={password}
          onChange={(event) => setPassword(event.target.value)}
          error={error}
        />
        {error &&
        <DialogContentText>
          Log in unsuccessful
        </DialogContentText>}
      </DialogContent>
      <DialogActions>
        <Button onClick={() => setOpen(false)}>Cancel</Button>
        <Button onClick={checkLogin}>Log in</Button>
      </DialogActions>
    </Dialog>
  );
};

export default LoginDialog;