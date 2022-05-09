import React, {useMemo, useState} from 'react';

export const UserContext = React.createContext({
    user: {username: '', password: ''},
    setUser: (user: {username: string, password: string}) => {}
  });

interface UserContextProps {
  children: React.ReactChild,
}

const UserContextProvider = ({children}: UserContextProps) => {
  const [user, setUser] = useState({username: '', password: ''});
  const value = useMemo(
    () => ({ user, setUser }),
    [user]
  );
  return (
    <UserContext.Provider value={value}>
      {children}
    </UserContext.Provider>
  );
};

export default UserContextProvider;