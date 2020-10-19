let storage: Storage = localStorage

const setItem = async (name: string, value: string): Promise<void> => {
  return storage.setItem(name, value)
}

const getItem = async (name: string): Promise<string> => {
  return storage.getItem(name) || ''
}

const setStorage = (instance: Storage): void => {
  storage = instance
}

export default { setStorage, setItem, getItem }
