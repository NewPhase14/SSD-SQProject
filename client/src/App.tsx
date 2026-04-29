import { useState } from 'react'
import Login from './components/Login'
import './App.css'

function App() {
  const [count, setCount] = useState(0)

  return (
      <Login></Login>
  )
}

export default App
