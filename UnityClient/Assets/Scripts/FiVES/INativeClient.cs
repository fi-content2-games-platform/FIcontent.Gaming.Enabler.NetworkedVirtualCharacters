using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NativeClient
{
    public interface INativeClient
    {
        /// <summary>
        /// List of all Entities contained in the world
        /// </summary>
        List<EntityInfo> World { get; }

        /// <summary>
        /// Fired when the client has successfully established an authenticated connection
        /// </summary>
        event EventHandler Authenticated;

        /// <summary>
        /// Fired when an entity created on one client was successfully created on the server.
        /// Contains EntityInfo and the Guid that the server assigned to the new entity in the
        /// EventArgs
        /// </summary>
        event EventHandler<EntityCreatedEventArgs> EntityCreated;

        /// <summary>
        /// Fired every time the client receives information about world updates from the server. UpdateEventArgs
        /// contain list of updates that consist of Entity Guid, updated component, attribute and the new values
        /// </summary>
        event EventHandler<UpdateEventArgs> ReceivedUpdate;

        /// <summary>
        /// Fired every time the client received info about a new object from the server.
        /// </summary>
        event EventHandler<NewObjectEventArgs> ReceivedNewObject;

        /// <summary>
        /// Creates a new Entity at origin
        /// </summary>
        void CreateNewEntity();
        
        /// <summary>
        /// Creates a new Entity at a given position
        /// </summary>
        /// <param name="Position">Position at which the entity shall be created</param>
        void CreateEntityAt(Vector3 Position);

        /// <summary>
        /// Moves the entity according to the Position stored in EntityInfo
        /// </summary>
        /// <param name="info">EntityInfo describing the entity</param>
        void MoveEntity(EntityInfo info);

        /// <summary>
        /// Rotates the entity according to the Position stored in EntityInfo
        /// </summary>
        /// <param name="info">EntityInfo describing the entity</param>
        void RotateEntity(EntityInfo info);

        /// <summary>
        /// Updates Boneinformation for an entity
        /// </summary>
        /// <param name="info">EntityInfo containing the new BonePositions</param>
        // TODO: Implement
        //void UpdateBones(EntityInfo info);
    }
}
