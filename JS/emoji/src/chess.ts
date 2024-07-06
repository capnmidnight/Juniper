import { G } from "./EmojiGroup";
import {
    blackChessBishop,
    blackChessKing,
    blackChessKnight,
    blackChessPawn,
    blackChessQueen,
    blackChessRook,
    whiteChessBishop,
    whiteChessKing,
    whiteChessKnight,
    whiteChessPawn,
    whiteChessQueen,
    whiteChessRook
} from ".";

export const whiteChessPieces = /*@__PURE__*/ (function () {
    return G(whiteChessKing.value + whiteChessQueen.value + whiteChessRook.value + whiteChessBishop.value + whiteChessKnight.value + whiteChessPawn.value, "White Chess Pieces", {
        width: "auto",
        king: whiteChessKing,
        queen: whiteChessQueen,
        rook: whiteChessRook,
        bishop: whiteChessBishop,
        knight: whiteChessKnight,
        pawn: whiteChessPawn
    });
})();

export const blackChessPieces = /*@__PURE__*/ (function () {
    return G(blackChessKing.value + blackChessQueen.value + blackChessRook.value + blackChessBishop.value + blackChessKnight.value + blackChessPawn.value, "Black Chess Pieces", {
        width: "auto",
        king: blackChessKing,
        queen: blackChessQueen,
        rook: blackChessRook,
        bishop: blackChessBishop,
        knight: blackChessKnight,
        pawn: blackChessPawn
    });
})();

export const chessPawns = /*@__PURE__*/ (function () {
    return G(whiteChessPawn.value + blackChessPawn.value, "Chess Pawns", {
        width: "auto",
        white: whiteChessPawn,
        black: blackChessPawn
    });
})();

export const chessRooks = /*@__PURE__*/ (function () {
    return G(whiteChessRook.value + blackChessRook.value, "Chess Rooks", {
        width: "auto",
        white: whiteChessRook,
        black: blackChessRook
    });
})();

export const chessBishops = /*@__PURE__*/ (function () {
    return G(whiteChessBishop.value + blackChessBishop.value, "Chess Bishops", {
        width: "auto",
        white: whiteChessBishop,
        black: blackChessBishop
    });
})();

export const chessKnights = /*@__PURE__*/ (function () {
    return G(whiteChessKnight.value + blackChessKnight.value, "Chess Knights", {
        width: "auto",
        white: whiteChessKnight,
        black: blackChessKnight
    });
})();

export const chessQueens = /*@__PURE__*/ (function () {
    return G(whiteChessQueen.value + blackChessQueen.value, "Chess Queens", {
        width: "auto",
        white: whiteChessQueen,
        black: blackChessQueen
    });
})();

export const chessKings = /*@__PURE__*/ (function () {
    return G(whiteChessKing.value + blackChessKing.value, "Chess Kings", {
        width: "auto",
        white: whiteChessKing,
        black: blackChessKing
    });
})();

export const chess = /*@__PURE__*/ (function () {
    return G("Chess Pieces", "Chess Pieces", {
        width: "auto",
        white: whiteChessPieces,
        black: blackChessPieces,
        pawns: chessPawns,
        rooks: chessRooks,
        bishops: chessBishops,
        knights: chessKnights,
        queens: chessQueens,
        kings: chessKings
    });
})();
