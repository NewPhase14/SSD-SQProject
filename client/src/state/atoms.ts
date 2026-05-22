// src/state/auth.ts
import { atom } from "jotai";

export const JwtAtom = atom<string | null>(localStorage.getItem("token"));

export const setJwtAtom = atom(
    null,
    (_get, set, jwt: string | null) => {
        if (jwt) {
            localStorage.setItem("token", jwt);
        } else {
            localStorage.removeItem("token");
        }
        set(JwtAtom, jwt);
    }
);