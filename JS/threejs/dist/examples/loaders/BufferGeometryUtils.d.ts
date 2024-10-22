/**
 * @param {BufferAttribute}
 * @return {BufferAttribute}
 */
export function deepCloneAttribute(attribute: any): BufferAttribute;
export function deinterleaveAttribute(attribute: any): BufferAttribute;
export function deinterleaveGeometry(geometry: any): void;
import { BufferAttribute } from 'three';
export function computeMikkTSpaceTangents(geometry: any, MikkTSpace: any, negateSign?: boolean): any;
/**
 * @param  {Array<BufferGeometry>} geometries
 * @param  {Boolean} useGroups
 * @return {BufferGeometry}
 */
export function mergeGeometries(geometries: Array<BufferGeometry>, useGroups?: boolean): BufferGeometry;
export function mergeBufferGeometries(geometries: any, useGroups?: boolean): BufferGeometry;
/**
 * @param {Array<BufferAttribute>} attributes
 * @return {BufferAttribute}
 */
export function mergeAttributes(attributes: Array<BufferAttribute>): BufferAttribute;
export function mergeBufferAttributes(attributes: any): BufferAttribute;
/**
 * @param {Array<BufferAttribute>} attributes
 * @return {Array<InterleavedBufferAttribute>}
 */
export function interleaveAttributes(attributes: Array<BufferAttribute>): Array<InterleavedBufferAttribute>;
/**
 * @param {Array<BufferGeometry>} geometry
 * @return {number}
 */
export function estimateBytesUsed(geometry: Array<BufferGeometry>): number;
/**
 * @param {BufferGeometry} geometry
 * @param {number} tolerance
 * @return {BufferGeometry}
 */
export function mergeVertices(geometry: BufferGeometry, tolerance?: number): BufferGeometry;
/**
 * @param {BufferGeometry} geometry
 * @param {number} drawMode
 * @return {BufferGeometry}
 */
export function toTrianglesDrawMode(geometry: BufferGeometry, drawMode: number): BufferGeometry;
/**
 * Calculates the morphed attributes of a morphed/skinned BufferGeometry.
 * Helpful for Raytracing or Decals.
 * @param {Mesh | Line | Points} object An instance of Mesh, Line or Points.
 * @return {Object} An Object with original position/normal attributes and morphed ones.
 */
export function computeMorphedAttributes(object: Mesh | Line | Points): Object;
export function mergeGroups(geometry: any): any;
export function toCreasedNormals(geometry: any, creaseAngle?: number): any;
import { BufferGeometry } from 'three';
import { InterleavedBufferAttribute } from 'three';
//# sourceMappingURL=BufferGeometryUtils.d.ts.map