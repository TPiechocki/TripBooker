import React, {useEffect, useState} from 'react';
import {request} from "../api/request";
import {Box, Container} from "@mui/material";

const Console = () => {
  const [logs, setLogs] = useState<{ timestamp: string, description: string }[]>([])
  useEffect(() => {
    request('GET', 'Updates')
      .then(response => {
        setLogs(response.updates);
      })
  }, [])
  return (
    <Box
      sx={{
        height: '100vh',
        width: '100vw',
        background: 'black',
        color: '#00ff00'
      }}
    >
      <Container
        sx={{
          display: 'flex',
          flexDirection: 'column-reverse',
        }}
      >
        {logs.map(log => (
          <Box key={log.timestamp} sx={{my: 0.5}}>{`${log.timestamp}>:\t${log.description}`}</Box>
        ))}
      </Container>
    </Box>
  );
};

export default Console;